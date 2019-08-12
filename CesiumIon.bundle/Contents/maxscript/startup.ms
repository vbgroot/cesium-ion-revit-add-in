(
	print "Loading Cesium ion menu..."
	fn findExportMenu =
	(
		local fileMenu = menuMan.findMenu "&File"
		
		if fileMenu == undefined then return undefined
		
		local exportMenu
		for i = 1 to fileMenu.numItems() do
		(
			local item = fileMenu.getItem i
			local itemTitle = item.getTitle()
			if itemTitle == "&Export" then 
			(
				exportMenu = item
				exit
			)
		)
		
		return exportMenu.getSubMenu()
	)

	if menuMan.registerMenuContext 0x8d1d625 then
	(
		local ionParentMenu = findExportMenu() -- find default export menu 
		if ionParentMenu == undefined then -- fallback when menu cannot be found
		(
			local mainMenuBar = menuMan.getMainMenuBar()
			ionParentMenu = menuMan.createMenu "Cesium ion"
			local parentMenuItem = menuMan.createSubMenuItem "Cesium ion" ionParentMenu
			mainMenuBar.addItem parentMenuItem -1
		)
		local ionAction = menuMan.createActionItem "cesiumIonUpload" "Cesium ion"
		ionParentMenu.addItem ionAction -1
		menuMan.updateMenuBar()
	)
	print "Loaded Cesium ion menu!"

)