macroScript cesiumIonUpload
	category: "Cesium ion"
	buttonText: "Upload to Cesium ion"
	toolTip: "Cesium ion - Upload"
	icon: "cesium"
	autoUndoEnabled: false
(
	fileIn ((GetDir #publicExchangeStoreInstallPath)+ @"\CesiumIon.bundle\Contents\maxscript\core.ms")
	try(destroyDialog ::cesiumIonDialog) catch()
	createDialog ::cesiumIonDialog width: 275
)

macroScript cesiumIonLogout
	category: "Cesium ion"
	buttonText: "Logout of Cesium ion"
	toolTip: "Cesium ion - Logout"
	icon: "cesium"
	autoUndoEnabled: false
(
	fileIn ((GetDir #publicExchangeStoreInstallPath)+ @"\CesiumIon.bundle\Contents\maxscript\core.ms")
	::cesiumIon.logout()
)