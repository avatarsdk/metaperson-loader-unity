# MetaPerson Loader For Unity
The package enables loading MetaPerson avatars in Unity in GLB/GLTF format.

## Requirements
 * Unity 2021.3.19f1 or a more recent version
 * Built-in render pipeline
 * Linear color space for better rendering quality
 * Supported platforms: Windows, MacOS, Android, iOS, WebGL
 
## Getting Started
You have two options to get started: you can import this package and the sample via Unity Package Manager or clone the repository and run the sample from it.

### Import Via Package Manager
1\. Open *Window->Package Manager*, click the **+** icon in the top left corner, and select **Add Package From Git URL**.

![Add Package From Git Url](./Documentation~/Images/add_package_from_git_url.jpg "Add Package From Git Url")

2\. Provide the Git URL of this project:

`https://github.com/avatarsdk/metaperson-loader-unity.git`

3\. Import **MetaPerson Loader Sample** as well.

![Import Sample](./Documentation~/Images/import_sample.jpg "Import Sample")

4\. Open the `Assets/Samples/MetaPerson Loader/[ver]/MetaPerson Loader Sample/Scenes/MetaPersonLoaderSample.unity` scene.

5\. Run the scene and click the "Load Avatar" button.

6\. The avatar will be downloaded and added to the scene.

![MetaPerson model](./Documentation~/Images/metaperson_model.JPG "MetaPerson Model")

### Copy Repository And Run Sample Project
1\. Clone this repository to your computer.

2\. Open the project from the `metaperson-loader-unity\Samples~\MetaPersonLoaderSample` directory in Unity 2021.3.19f1 or a newer.

3\. Open the `Assets/AvatarSDK/MetaPersonLoader/Sample/Scenes/MetaPersonLoaderSample.unity` scene.

4\. Run the scene and click the "Load Avatar" button.

5\. The avatar will be downloaded and added to the scene.

#### Changing The Loaded Model
To load another model, provide a URL to the GLB/GLTF model or a ZIP archive containing such a model. Update the **Model Url** field in the **Meta Person Sample** script.

![Change model URL](./Documentation~/Images/change_model_url.JPG "Change Model URL")

<!---
|||||     A note from the scene user who was confused for a while ;)
---->

##### Serving your own Meta Avatar Model

The model URL does not have to be provided by `metaperson.avatarsdk.com`.
You may set the URL field to your link to the GLB file. For example:

```
// In fact, it may be more convenient to set the public field value in the
// Inspector window of the SceneHandler GameObject in the Unity Editor.
//
public string modelUri = "http://example.yourgame.com/route/of/the/entrypoint/to/your/glbfile/filename.glb";
```
The Metaperson Loader Unity Scene Does not require clients to provide `Client_ID` or `Secret_Key`, send additional special HTTP request headers, or make any extra effort beyond making GLB files accessible by the URL they provide.
The request is sent using the Scene script, a basic GET request.
Following up the custom GLB file URL, the `example.yourgame.com` Server can display the Log, similar to the following:

```
"GET /route/of/the/entrypoint/to/your/glbfile/filename.glb HTTP/1.1" 200 14585949 "-" "UnityPlayer/2021.3.11f1 (UnityWebRequest/1.0, libcurl/7.84.0-DEV)"
```
Where `14585949` is the size of the GLB file.

Of course, creating a GLB model is also vital for this example.
The GLB file can be created using the modified [Meta Person Creator](https://metaperson.avatarsdk.com/).
It can be integrated using your website or localhost web app HTML's `<iframe>`.
Using Meta person [JS API](https://docs.metaperson.avatarsdk.com/js_api)), the client of IFRAME can set the export parameter to `GLB`.
Using customised Meta Person Creator and Meta Person Loader Scene, one can create unique projects with realistic human avatars created with minimal labour.

<!---
|||||    End of the little note.
---->

## How It Works
This package uses [glTFast](https://docs.unity3d.com/Packages/com.unity.cloud.gltfast@6.0/manual/index.html) to load MetaPerson models in **GLB/GLTF** format.

To load a MetaPerson model, follow these steps:
1. Create an empty object in the scene.
2. Add a [MetaPersonLoader](./Runtime/Scripts/MetaPersonLoader.cs) component to this object.
3. Specify the **Avatar Object** field, which is the parent object of the instantiated avatar.
4. It's recommended to create a [MetaPersonMaterialGenerator](./Runtime/Scripts/MetaPersonMaterialGenerator.cs) component and assign it to the **Material Generator** field of the [MetaPersonLoader](./Runtime/Scripts/MetaPersonLoader.cs).
This component provides preconfigured materials and sets up avatar textures. When **Material Generator** isn't specified, default materials are used.
![MetaPerson Loader](./Documentation~/Images/meta_person_loader.JPG "MetaPerson Loader").
5. Call the **LoadModelAsync** method of the [MetaPersonLoader](./Runtime/Scripts/MetaPersonLoader.cs) by passing a URL to a model.
```c#
bool isModelLoaded = await metaPersonLoader.LoadModelAsync(modelUrl, p => Debug.LogFormat("Downloading avatar: {0}%", (int)(p * 100)));
```

### MetaPersonLoader Properties
 * **Model Url**: a URL to the GLB/GLTF or a ZIP with a MetaPerson model.
 * **Avatar Object**: a parent object of the instantiated avatar. If it isn't specified, the MetaPersonLoader's object is a parent of the avatar.
 * **Material Generator**: provides avatar's materials.
 * **Cache Models**: if true, the downloaded model is saved to the [persistent storage](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html) for caching.
 * **Configure Animator**: if true, an Animator component is added to the avatar.
 * **Animator Controller**: runtime animator controller assigned to the Animator.

### MetaPersonMaterialGenerator
The [MetaPersonMaterialGenerator](./Runtime/Scripts/MetaPersonMaterialGenerator.cs) component offers preconfigured materials to be used in place of the default materials provided by [glTFast](https://docs.unity3d.com/Packages/com.unity.cloud.gltfast@6.0/manual/index.html). It also ensures that textures are set up correctly.

This component includes the following materials:
 * **Default Material**: This material is used by default.
 * **Body Material**: This material is used to render the avatar's body.
 * **Head Material**: This material is used to render the avatar's head.
 * **Eyelashes Material**: This material is used to render eyelashes.
 * **Cornea Material**: This material is used to render cornea.
 * **Eyeball Material**: This material is used to render eyeballs.
 * **Outfit Material**: This material is used to render outfits.
 * **Haircut Material**: This material is used to render haircuts.
 * **Glasses Material**: This material is used to render glasses.
 
This component lets you specify the template materials to meet your specific requirements.
You may also implement a custom version of the **MaterialGenerator**.

## How To Integrate MetaPerson Creator Into Your Application
[MetaPerson Creator](https://metaperson.avatarsdk.com/)  web page can be integrated into your application, allowing your clients to create their custom avatars and import them into your product.

Unity samples:
 * [Windows and macOS](./Documentation~/MetaPersonCreatorDesktopIntegration.md)
 * [Android and iOS](./Documentation~/MetaPersonCreatorMobileIntegration.md)
 * [Android and iOS via Vuplex webview](./Documentation~/MetaPersonCreatorMobileIntegrationViaVuplex.md)
 * [VR Quest](https://github.com/avatarsdk/metaperson-vr-quest-sample)
 * [WebGL - integration via IFrame](./Documentation~/MetaPersonCreatorWebGLIFrameIntegration.md)
 * [WebGL - integration via WebView](./Documentation~/MetaPersonCreatorWebGLWebViewIntegration.md)
 
Native samples:
 * [Android](https://github.com/avatarsdk/metaperson-android-sample)
 * [iOS](https://github.com/avatarsdk/metaperson-ios-sample)

Other samples:
 * [Integration with body tracking from the Movement SDK for Meta Quest](https://github.com/avatarsdk/metaperson-quest-movement-sdk-sample)
 * [Unity Multiplayer Photon Sample](https://github.com/avatarsdk/metaperson-unity-photon-sample)

## Support
If you have any questions or issues with the plugin, don't hesitate to contact us at <support@avatarsdk.com>.
