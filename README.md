# Minecraft Clone

![screenshot.png](./doc/screenshot.png)

## Middlewares/Tools

* Unity 2019.3.0f6
* ImageMagick

## Setup

### Assets

Please import the following assets from Unity Asset Store:

* [Standard Assets (for Unity 2017.3) - Asset Store](https://assetstore.unity.com/packages/essentials/asset-packs/standard-assets-for-unity-2017-3-32351)
* [UniRx - Reactive Extensions for Unity - Asset Store](https://assetstore.unity.com/packages/tools/integration/unirx-reactive-extensions-for-unity-17276)

#### Fix Standard Assets errors on Unity 2019.3

```sh
cp Patches/Standard\ Assets/Utility/*.patch Assets/Standard\ Assets/Utility/
cd Assets/Standard\ Assets/Utility/
patch -u < *.patch
```

**See:** [Unity 2019.3 で Standard Assets をインポートした際に発生する 'GUITexture' is obsolete というエラーの対処方法 - Unity Connect](https://connect.unity.com/p/standard-assets-guitexture-and-guitext-are-obsolete)

### Block texture generation

1. Move `Assets/MinecraftClone/Textures` directory
2. Enter `rake` command

## Documentation

* [Minecraft clone design | mrk21 Kibela](https://mrk21.kibe.la/shared/entries/3d340747-4142-4568-9d78-d0ce494ca9d7)
* [Minecraft clone memo | mrk21 Kibela](https://mrk21.kibe.la/shared/entries/294c5ea1-70db-40ca-a455-7f3266158789)