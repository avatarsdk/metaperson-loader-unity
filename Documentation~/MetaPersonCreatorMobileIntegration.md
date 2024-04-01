# MetaPerson Creator - Integration Into Android And iOS Unity Application 
This sample demonstrates how to integrate the [MetaPerson Creator](https://mobile.metaperson.avatarsdk.com/generator) web page into **Android** or **iOS** applications and export an avatar from it.

## Requirements
 * Unity 2021.3.19f1 or a more recent version
 * Built-in render pipeline
 * Linear color space for better rendering quality

## Getting Started
**1\.** Open the sample scene. 

You can get this sample via Unity Package Manager or clone the repository and run the sample from it.

### Get the sample via Package Manager.

 * Open *Window->Package Manager*, click on the **+** icon in the top left corner and select **Add Package From Git URL**.
 
![Add Package From Git Url](./Images/add_package_from_git_url.jpg "Add Package From Git Url")

 * Provide the Git URL of this project:

`https://github.com/avatarsdk/metaperson-loader-unity.git`

 * Import **MetaPerson Creator Mobile Integration Sample** as well.

![Import Sample](./Images/import_mobile_integration_sample.jpg "Import Sample")

 * Open the `Assets/Samples/MetaPerson Loader/x.y.z/MetaPerson Creator Mobile Integration Sample/Scenes/MetaPersonCreatorMobileIntegrationSample.unity` scene.

### Get the sample from the repository.

 * Clone this repository to your computer.

 * Open the project from `metaperson-loader-unity\Samples~\MetaPersonCreatorMobileIntegrationSample` directory in Unity 2021.3.19f1 or a newer.

 * Open the `Assets/AvatarSDK/MetaPerson/MobileIntegrationSample/Scenes/MetaPersonCreatorMobileIntegrationSample.unity` scene.

**2\.** Find **SceneHandler** object and provide your [Account Credentials](AccountCredentials.md).

![Account Credentials](./Images/account_credentials_mobile.jpg "Account Credentials")

**3\.** Build and run application.

**4\.** Press the **Get Avatar** button. MetaPerson Creator page will be shown.

**5\.** Select any of the sample avatars or create your own, customize it, and press the **Export** -> **GLB** button.

![Export Avatar](./Images/mobile_export_avatar.jpg "Export Avatar")

**6\.** The avatar will be exported and added to the scene.

![Avatar On Scene](./Images/mobile_avatar_on_scene.jpg "Avatar On Scene")

## How It Works
The sample shows the [MetaPerson Creator](https://mobile.metaperson.avatarsdk.com/generator) page in a webview component and communicates with it via [JS API](https://docs.metaperson.avatarsdk.com/js_api.html).

Here's basic overview of how it works:

1\. Load the following page in a WebView component: `https://mobile.metaperson.avatarsdk.com/generator`.

2\. Once the page is loaded, the following JavaScript code is executed. It subscribes to `message` events to be ready to receive events from the MetaPerson Loader and call the `sendConfigurationParams` method.

```js
function sendConfigurationParams() {
  console.log('sendConfigurationParams - called');

  const CLIENT_ID = '" + credentials.clientId + @"';
  const CLIENT_SECRET = '" + credentials.clientSecret + @"';

  let authenticationMessage = {
    'eventName': 'authenticate',
    'clientId': CLIENT_ID,
    'clientSecret': CLIENT_SECRET
  };
  window.postMessage(authenticationMessage, '*');

  let exportParametersMessage = {
    'eventName': 'set_export_parameters',
    'format': 'glb',
    'lod': 2,
    'textureProfile': '1K.jpg'
  };
  window.postMessage(exportParametersMessage, '*');

  let uiParametersMessage = {
    'eventName': 'set_ui_parameters',
    'isExportButtonVisible' : true,
    'isLoginButtonVisible': true
  };
  window.postMessage(uiParametersMessage, '*');
}

function onWindowMessage(evt) {
  if (evt.type === 'message') {
    if (evt.data?.source === 'metaperson_creator') {
      let data = evt.data;
      let evtName = data?.eventName;
      if (evtName === 'unity_loaded' ||
        evtName === 'mobile_loaded') {
        console.log('got mobile_loaded event');
        sendConfigurationParams();
      } else if (evtName === 'model_exported') {
        console.log('got model_exported event');
        const params = new URLSearchParams();
        params.append('url', data.url);
        params.append('gender', data.gender);
        params.append('avatarCode', data.avatarCode);
        window.location.href = 'uniwebview://model_exported?' + params.toString();
      }
    }
  }
}
window.addEventListener('message', onWindowMessage);

sendConfigurationParams();
```
3\. The `sendConfigurationParams` method posts messages with configration parameters to the MetaPerson Creator. It specifies **authentication credentials**, **export** and **UI** parameters.

4\. When the MetaPerson Creator exports an avatar, it sends the `model_exported` event that contains a URL to a GLB file with the avatar.

5\. The URL of the exported avatar is passed to the app and the model is loaded to the scene by using **MetaPerson Loader**.
```js
if (evtName === 'model_exported') {
  console.log('got model_exported event');
  const params = new URLSearchParams();
  params.append('url', data.url);
  params.append('gender', data.gender);
  params.append('avatarCode', data.avatarCode);
  window.location.href = 'uniwebview://model_exported?' + params.toString();
}
```
Implementation details can be found in the [MobileUnitySampleHandler.cs script](./../Samples~/MetaPersonCreatorMobileIntegrationSample/Assets/AvatarSDK/MetaPerson/MobileIntegrationSample/Scripts/MobileUnitySampleHandler.cs).

More information about JS API parameters can be found here: https://docs.metaperson.avatarsdk.com/js_api.html

## Support
If you have any questions or issues with the sample, please contact us <support@avatarsdk.com>.
