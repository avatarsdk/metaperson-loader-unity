# MetaPerson Loader Unity Sample
This sample demonstrates how to load MetaPerson models in Unity.

## Requirements
 * Unity 2021.3.19f1 or a more recent version
 * Built-in render pipeline
 * Supported platforms: Windows, MacOS, Android, iOS, WebGL
 
## Getting Started
1. Clone this repository to your computer.
2. Open the project in Unity 2021.3.19f1 or a newer version.
3. Open the `Assets/AvatarSDK/MetaPerson/Sample/Scenes/MetaPersonLoaderSample.unity` scene.
4. Run the scene and click the "Load Avatar" button.
5. The avatar will be downloaded and added to the scene.
![MetaPerson model](./Documentation~/Images/metaperson_model.JPG "MetaPerson Model")

To load another model, you need to provide a URL to the GLB/GLTF model or to a ZIP archive containing such a model. Update the **Model Url** field in the **Meta Person Sample** script.
![Change model URL](./Documentation~/Images/change_model_url.JPG "Change Model URL")

## How It Works
This sample uses [glTFast](https://github.com/atteneder/glTFast) plugin to load MetaPerson models in **GLB/GLTF** format.<br/>
<br/>
To load a MetaPerson model, follow these steps:
1. Create an empty object in the scene.
2. Add a [MetaPersonLoader](./Assets/AvatarSDK/MetaPerson/Loader/Scripts/MetaPersonLoader.cs) component to this object.
3. Specify the **Avatar Object** field, which is the parent object of the instantiated avatar.
4. It's recommended to create a [MetaPersonMaterialGenerator](./Assets/AvatarSDK/MetaPerson/Loader/Scripts/MetaPersonMaterialGenerator.cs) component and assign it to the **Material Generator** field of the [MetaPersonLoader](./Assets/AvatarSDK/MetaPerson/Loader/Scripts/MetaPersonLoader.cs).
This component provides preconfigured materials and sets up avatar textures. When **Material Generator** isn't provided, default materials are used.
![MetaPerson Loader](./Documentation~/Images/meta_person_loader.JPG "MetaPerson Loader").
5. Call the **LoadModelAsync** method of the [MetaPersonLoader](./Assets/AvatarSDK/MetaPerson/Loader/Scripts/MetaPersonLoader.cs) by passing a URL to a model.
 ```c#
bool isModelLoaded = await metaPersonLoader.LoadModelAsync(modelUrl, p => Debug.LogFormat("Downloading avatar: {0}%", (int)(p * 100)));
```

### MetaPersonMaterialGenerator
The [MetaPersonMaterialGenerator](./Assets/AvatarSDK/MetaPerson/Loader/Scripts/MetaPersonMaterialGenerator.cs) component offers preconfigured materials to be used in place of the default materials provided by [glTFast](https://github.com/atteneder/glTFast). It also ensures that textures are set up correctly.<br/>

This component includes the following materials:
 * **Default Material**: This material is utilized for rendering Head, Body, Eyes, Mouth, and Outfits meshes. It is based on the **Standard** opaque shader.
 * **Eyelashes Material**: This material is utilized for rendering Eyelashes meshes. It is based on the **Standard** fade shader.
 * **Haircut Material**: This material is utilized for rendering Haircuts meshes. The shader for this material can be found [here](./Assets/AvatarSDK/MetaPerson/Loader/Shaders/haircuts/avatar_sdk_haircut_standard.shader).
 * **Glasses Material**: This material is utilized for rendering Glasses meshes. It is based on the [double-sided **Standard** fade shader](./Assets/AvatarSDK/MetaPerson/Loader/Shaders/avatar_sdk_standard_double_sided.shader).
You have the flexibility to modify these template materials to suit your specific needs, or you can implement a custom version of the **MaterialGenerator**.

## How To Get MetaPerson model In GLB Format
Exporting models from [MetaPerson Creator](https://metaperson.avatarsdk.com/) requires having an AvatarSDK developer account.<br/>
Follow these steps to get it:
* Get an AvatarSDK developer account at https://accounts.avatarsdk.com/developer/signup/
* Create an application with Client credentials Authorization Grant at https://accounts.avatarsdk.com/developer/
* Copy the `App Client ID` and `App Client Secret` from the Client Access application at https://accounts.avatarsdk.com/developer/
![App Client Credentials](./Documentation~/Images/credentials.JPG "App Client Credentials") 
* Download a source code of the sample page with **MetaPerson Creator**: https://metaperson.avatarsdk.com/business.html
* Modify the source code of this page by providing your `App Client ID` and `App Client Secret` values.
![Business Integration](./Documentation~/Images/business_integration_credentials.JPG "Business Integration")
* Now you can open this page in a browser, open the creator and export an avatar in GLB format.

Find out more information about business integration at https://docs.metaperson.avatarsdk.com/business_integration.html. 

## Support
If you have any questions or issues with the plugin, please contact us <support@avatarsdk.com>.