# Minecraft Clone

![screenshot.png](./doc/screenshot.png)

## Dependencies

### Middlewares/Tools

* Unity: 2019.3.0f6
* ImageMagick
* Ruby
* direnv

### Assets

* [Standard Assets](https://assetstore.unity.com/packages/essentials/asset-packs/standard-assets-for-unity-2017-3-32351)
* [UniRx](https://assetstore.unity.com/packages/tools/integration/unirx-reactive-extensions-for-unity-17276): 7.1.0

## Setup

```sh
# Set your config
cp .envrc.local.sample .envrc.local
vi .envrc.local
direnv allow .

# Activation
unity -createManualActivationFile
## Upload the manual activation file on https://license.unity3d.com/manual
unity -manualLicenseFile Unity_v2019.x.ulf

# Import Assets
unity -executeMethod ImportAssets.Import
apply_patch

# Generate Texture
generate_texture
```

## Development

### Unity batch commands

```sh
# Create *.ulf file
unity -createManualActivationFile

# Activation
unity -manualLicenseFile Unity_v2019.x.ulf

# Import assets
unity -executeMethod ImportAssets.Import /basePath /path/to/assets

# Build app
unity -executeMethod Builder.Build /platform webgl
```

## Documentation

* [Minecraft clone design | mrk21 Kibela](https://mrk21.kibe.la/shared/entries/3d340747-4142-4568-9d78-d0ce494ca9d7)
* [Minecraft clone memo | mrk21 Kibela](https://mrk21.kibe.la/shared/entries/294c5ea1-70db-40ca-a455-7f3266158789)
