# MetaPerson Creator - Integration Into Windows And MacOS Unity Application 
This sample demonstrates how to integrate the [MetaPerson Creator](https://metaperson.avatarsdk.com/) web page into **Windows** or **macOS** applications and export an avatar from it.

**Note:** A special WebView component is required to display web pages inside a Unity application. This sample uses the [Vuplex Web View](https://store.vuplex.com/webview/windows-mac) plugin, which is paid.
You can also choose any other 3rd party solution with a WebView component.

## Getting Started
**1\.** Open the sample scene. 

You can get this sample via Unity Package Manager or clone the repository and run the sample from it.

### Get the sample via Package Manager.

 * Add a new Scoped Registry to *Project Settings -> Package Manager*. This registry is required to import the [glTFast](https://github.com/atteneder/glTFast) package as a dependency.

```json
"scopedRegistries": [
  {
    "name": "OpenUPM",
    "url": "https://package.openupm.com",
    "scopes": [
      "com.atteneder"
    ]
  }
]
```
![Add Scoped Registry](./Images/add_scoped_registry.JPG "Add Scoped Registry")

 * Open *Window->Package Manager*, click on the **+** icon in the top left corner and select **Add Package From Git URL**.
 
![Add Package From Git Url](./Images/add_package_from_git_url.jpg "Add Package From Git Url")

 * Provide the Git URL of this project:

`https://github.com/avatarsdk/metaperson-loader-unity.git`

 * Import **MetaPerson Creator Desktop Integration Sample** as well.

![Import Sample](./Images/import_desktop_integration_sample.jpg "Import Sample")

 * Open the `Assets/Samples/MetaPerson Loader/0.1.1/MetaPerson Creator Desktop Integration Sample/Scenes/MetaPersonCreatorDesktopIntegrationSample.unity` scene.

### Get the sample from the repository.

 * Clone this repository to your computer.

 * Open the project from `metaperson-loader-unity\Samples~\MetaPersonCreatorDesktopIntegrationSample` directory in Unity 2021.3.19f1 or a newer.

 * Open the `Assets/AvatarSDK/MetaPerson/DesktopIntegrationSample/Scenes/MetaPersonCreatorDesktopIntegrationSample.unity` scene.
 
**2\.** Import [Vuplex plugin](https://store.vuplex.com/webview/windows-mac) into the project.

**3\.** Find **SceneHandler** object and provide your [Account Credentials](#account-credentials).

![Account Credentials](./Images/account_credentials.jpg "Account Credentials")

**4\.** Run the scene.

**5\.** Press the **Get Avatar** button. MetaPerson Creator page will be shown.

**6\.** Select any of the sample avatars or create your own, customize it and press **Export** button.

![Export Avatar](./Images/export_avatar.JPG "Export Avatar")

**7\.** The avatar will be exported and added to the scene.

![Avatar On Scene](./Images/avatar_on_scene.JPG "Avatar On Scene")

## Account Credentials
To export models from the [MetaPerson Creator](https://metaperson.avatarsdk.com/), you'll need AvatarSDK developer account credentials. Follow these steps to obtain them:

1. **Create an AvatarSDK Developer Account:**
   Visit the [AvatarSDK Developer Signup page](https://accounts.avatarsdk.com/developer/signup/) to create your AvatarSDK developer account. If you already have an account, you can skip this step.

2. **Create an Application:**
   After successfully registering or logging in to your AvatarSDK developer account, go to the [Developer Dashboard](https://accounts.avatarsdk.com/developer/). Here, create a new application. 

3. **Retrieve Your App Client ID and App Client Secret:**
   Once your application is created, you can obtain your **App Client ID** and **App Client Secret** from the Developer Dashboard.

![App Client Credentials](./Documentation~/Images/credentials.JPG "App Client Credentials")


Find out more information about business integration at https://docs.metaperson.avatarsdk.com/business_integration.html. 

## How It Works

## Export Parameters

## Support
If you have any questions or issues with the plugin, please contact us <support@avatarsdk.com>.
