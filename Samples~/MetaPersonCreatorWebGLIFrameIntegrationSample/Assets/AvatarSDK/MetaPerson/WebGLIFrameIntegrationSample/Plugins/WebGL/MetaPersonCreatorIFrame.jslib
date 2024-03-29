mergeInto(LibraryManager.library, 
{
	showMetaPersonCreator: function(clientIdStrPtr, clientSecretStrPtr, modelUrlReceiverObjectNamePtr, modelUrlReceiverMethodNamePtr) 
	{
		var clientIdStr = UTF8ToString(clientIdStrPtr);
		var clientSecretStr = UTF8ToString(clientSecretStrPtr);
		var modelUrlReceiverObjectName = UTF8ToString(modelUrlReceiverObjectNamePtr);
		var modelUrlReceiverMethodName = UTF8ToString(modelUrlReceiverMethodNamePtr);

		if (!this.isInitialized)
		{	
			window.addEventListener("message", onWindowMessage);
			this.isInitialized = true;
		}

		var metaPersonCreatorContainer = document.createElement('div');
		metaPersonCreatorContainer.setAttribute('id', 'metaperson-creator-container');
		metaPersonCreatorContainer.innerHTML = `
			<label id="authentication_status_label" style="position:absolute; top:-20px; left:2px;">Athentication Status: Not Provided</label>
			<button id="metaperson-creator-close-button" type="button" style="position:absolute; top:-25px; right:2px; width:100px; height:25px">Close</button>
			<iframe id="metaperson-creator-iframe" src="https://metaperson.avatarsdk.com/dev/v1.13.1/v3/iframe.html" allow="fullscreen"></iframe>
		`;

		var unityContainer = document.getElementById('unity-container');
		unityContainer.insertBefore(metaPersonCreatorContainer, unityContainer.firstChild);

		var iframe = document.getElementById('metaperson-creator-iframe');
		var unityCanvas = document.getElementById('unity-canvas');
		iframe.width = unityCanvas.width;
		iframe.height = unityCanvas.height;
		unityCanvas.style.display = "none";
		metaPersonCreatorContainer.style.display = "block";

		var closeButton = document.getElementById('metaperson-creator-close-button');
		closeButton.onclick = closeMetaPersonCreator;

		function onWindowMessage(evt)
		{
			if (evt.type === "message") 
			{
				if (evt.data && evt.data.source === "metaperson_creator")
				{
					let data = evt.data;
					if (data.eventName)
					{
						switch (data.eventName) 
						{
							case "unity_loaded":
								configureMetaPersonCreator();
								break;
							case "model_exported":
								SendMessage(modelUrlReceiverObjectName, modelUrlReceiverMethodName, JSON.stringify(data));
								closeMetaPersonCreator();
								break;
							case "authentication_status":
								updateAuthenticationStatus(data.isAuthenticated, data.errorMessage);
								break;
						}
					}
				}
			}
		}

		function configureMetaPersonCreator() 
		{
			var authenticationLabel = document.getElementById('authentication_status_label');
			authenticationLabel.innerText = "Athentication Status: In Progress";

			var iframe = document.getElementById('metaperson-creator-iframe');

			let authenticationMessage = 
			{
				"eventName": "authenticate",
				"clientId": clientIdStr,
				"clientSecret": clientSecretStr
			};
			iframe.contentWindow.postMessage(authenticationMessage, "*");

			let exportParametersMessage = 
			{
				"eventName": "set_export_parameters",
				"format" : "glb",
				"lod" : 1,
				"textureProfile" : "1K.png",
				"useZip" : false
			};
			iframe.contentWindow.postMessage(exportParametersMessage, "*");

			let uiParametersMessage = 
			{
				"eventName": "set_ui_parameters",
				"isExportButtonVisible" : true,
				"closeExportDialogWhenExportComlpeted" : true,
				"isLoginButtonVisible" : true,
				"outfitsBlackList" : []
			};
			iframe.contentWindow.postMessage(uiParametersMessage, "*");
		}

		function closeMetaPersonCreator()
		{
			var unityCanvas = document.getElementById('unity-canvas');
			unityCanvas.style.display = "block";

			var metaPersonCreatorContainer = document.getElementById('metaperson-creator-container');
			metaPersonCreatorContainer.remove();
		}

		function updateAuthenticationStatus(isAuthenticated, errorMessage)
		{
			var authenticationLabel = document.getElementById('authentication_status_label');
			if (isAuthenticated) 
			{
				authenticationLabel.style.color = "green";
				authenticationLabel.innerText = "Athentication Status: OK";
			} 
			else 
			{
				authenticationLabel.style.color = "red";
				authenticationLabel.innerText = "Athentication Status: Error";
				console.error("Account was not authenticated: " + errorMessage);
			}
		}
	}
});