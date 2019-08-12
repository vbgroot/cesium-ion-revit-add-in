fn exportFBX directory: sysInfo.tempdir =
(
	local maxName = getfilenamefile maxfilename
	if maxName.count <= 0 then maxName = "default"
	local outName = directory + maxname + ".fbx"

	local manager = pluginManager
	manager.loadClass FBXEXPORTER

	FBXExporterSetParam "ASCII" False
	FBXExporterSetParam "FileVersion" "FBX201900"
	FBXExporterSetParam "UpAxis" "Z"
	FBXExporterSetParam "Animation" False
	FBXExporterSetParam "Cameras" True
	FBXExporterSetParam "Lights" True
	FBXExporterSetParam "PreserveInstances" True
	FBXExporterSetParam "ScaleFactor" 1.0
	FBXExporterSetParam "ShowWarnings" False
	FBXExporterSetParam "EmbedTextures" True
	FBXExporterSetParam "PushSettings" True
	exportFile (outName) #noprompt using:manager
	
	return outName
)

fn resolvePath relPath = (GetDir #publicExchangeStoreInstallPath) + @"\CesiumIon.bundle\Contents\" + relPath

struct cesiumIon
(
	private 
	fn loadAssembly = dotnet.loadAssembly (resolvePath @"binary\Cesium.Ion.dll"),
	
	fn setToken val = setINISetting (resolvePath @"config\ion.ini") "Ion" "Token" val,

	public
	fn getConfig key category: "Ion" = getINISetting (resolvePath @"config\ion.ini") category key,
	fn getToken = getConfig "Token",
		
	fn logout = setToken undefined,
		
	fn login open: true =
	(
		fn  OnAuthListener sender args =
		(
			print args.Token
			if args.Token != undefined then
			(
				setToken args.Token
			)
		)
		
		try (server.Dipose()) catch()
		loadAssembly()
		local Process = dotNetClass "System.Diagnostics.Process"
		local authenticator = dotNetObject "Cesium.Ion.IonAuthenticator" (getConfig "ClientID")
		local server = dotNetObject "Cesium.Ion.IonAuthServer"
		dotNet.setLifetimeControl authenticator #dotnet
		dotNet.setLifetimeControl server #dotnet

		dotNet.removeEventHandlers server "OnAuthListener"
		dotNet.addEventHandler server "OnAuthListener" OnAuthListener
		server.Listen authenticator

		local targetURL = authenticator.GetOAuthURL()
		
		if open then Process.Start targetURL
		return targetURL
	),
	
	fn upload targetFile projectName desc: "" attr: "" webp: false open: true =
	(
		loadAssembly()
		local Process = dotNetClass "System.Diagnostics.Process"
		local IonAssetAPI = dotNetClass "Cesium.Ion.IonAssetAPI"
		
		local token = getToken()
		local assetAPI = dotNetObject IonAssetAPI token "https://api.cesium.com/"
		
		local ionAsset = assetAPI.Create projectName desc attr webp
		local ionUpload = assetAPI.Upload ionAsset.Result targetFile
		local url = ionUpload.Result
		
		if open then Process.Start url
			return url
	)
)

rollout cesiumIonDialog "Cesium ion Upload"
(
	local width = 275
	local imgWidth = 200

	label spacer0 visible:false
	local bitmapLogo = openBitmap (resolvePath @"resources\ion_color_white.png") 
	local imgHeight = imgWidth * 0.17
	imgTag logo "Bitmap" width:imgWidth  height:imgHeight align:#center bitmap:bitmapLogo
	label spacer1 visible:false
	
	editText nameEditText "Name:" labelOnTop:true
	label nameErrorLabel "Error! Name is required!" visible: false
	editText descriptionEditText "Description:" labelOnTop:true height:75
	label spacer2 visible:false
	editText attributionEditText "Attribution:" labelOnTop:true
	label spacer3 visible:false
	checkbox webpCheckbox "Use WebP"
	label spacer4 visible:false
	local buttonWidth = ( width - 30 ) / 2
	button uploadButton "Upload" across:2 align:#left width:buttonWidth
	button cancelButton "Cancel" align:#right width:buttonWidth
	
	on uploadButton pressed do
	(
		if nameEditText.text.count > 0 then
		(
			nameErrorLabel.visible = false
			local token = cesiumIon.getToken()
			if token.count > 0 then
			(
				local outPath = exportFBX()
				cesiumIon.upload outPath nameEditText.text \
					desc: descriptionEditText.text attr: attributionEditText.text \
					webp: webpCheckbox.checked
				DestroyDialog cesiumIonDialog
			) else cesiumIon.login()
		) else nameErrorLabel.visible = true
	)
	
	on cancelButton pressed do DestroyDialog  cesiumIonDialog
)

print "Loaded Cesium core scripts!"