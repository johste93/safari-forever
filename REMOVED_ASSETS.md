# Removed Assets

This document lists everything that was removed from the repository before open-sourcing, why it was removed, and what a future maintainer would need to replace or re-acquire to make the project buildable again.

---

## Unity Plugins (client)

These were removed because they are commercial Asset Store products or third-party SDKs that cannot be redistributed.

### DOTween / DOTweenPro
- **What it is:** Tweening animation library for Unity. DOTween (free) and DOTweenPro (paid).
- **Removed folder:** `client/Assets/Demigiant/`
- **Where to get:** https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676 (free) — DOTweenPro requires a separate paid license from the same publisher.
- **Used by:** `Assets/_Assets/Code/UI/TransitionHole.cs`, `Assets/_Assets/Code/Toys/UnstableBlock.cs`, `Assets/_Assets/Code/Toys/Bubble.cs`, `Assets/_Assets/Code/MainMenu/ScreenScroller.cs`
- **Note:** The free version (DOTween) is redistributable under its own license, but DOTweenPro is not, so the entire folder was removed.

### Firebase Unity SDK
- **What it is:** Google Firebase SDK for Unity — App and Cloud Messaging packages.
- **Removed folders:** `client/Assets/Firebase/`, `client/GooglePackages/`
- **Packages removed:** `com.google.firebase.app-8.1.0`, `com.google.firebase.messaging-8.7.0`, `com.google.external-dependency-manager-1.2.168`
- **Where to get:** https://firebase.google.com/docs/unity/setup — requires a Firebase project and `GoogleService-Info.plist` (see Secrets section below).
- **Used by:** `Assets/_Assets/Code/UI/Shop.cs`, `Assets/_Assets/Code/IAP/IAPManager.cs`, `Assets/_Assets/Code/Editor/CustomBuilder/iOSBuildActions.cs`

### IOSGoodies
- **What it is:** Native iOS features plugin (camera, gallery, haptics, Face ID, etc.) by Nine Vast Studios. Version 2.4.0.
- **Removed folder:** `client/Assets/IOSGoodies/`
- **Where to get:** https://assetstore.unity.com/packages/tools/integration/ios-native-pro-iosgoodiestools-platform-68190 (paid)
- **Used by:** `Assets/_Assets/Code/UI/ShareCodeButton.cs`, `Assets/_Assets/Code/UI/ShareRecordingMenuWindow.cs`, `Assets/_Assets/Code/LevelInspector/LevelPublisher.cs`

### JsonDotNet (Newtonsoft.Json for Unity)
- **What it is:** Newtonsoft Json.NET Unity package. Version 2.0.1.
- **Removed folder:** `client/Assets/JsonDotNet/`
- **Where to get:** The free open-source version is at https://github.com/jilleJr/Newtonsoft.Json-for-Unity — the Asset Store version is paid.
- **Used by:** Numerous files across `Assets/_Assets/Code/` (JSON serialization throughout the app).

### NativeShareDialog
- **What it is:** Native share sheet plugin for iOS/Android (unknown publisher).
- **Removed folder:** `client/Assets/NativeShareDialog/`
- **Where to get:** Unknown — no license or publisher info was present. Check Unity Asset Store or search for alternatives such as NativeShare by yasirkula (https://github.com/yasirkula/UnityNativeShare).
- **Used by:** `Assets/_Assets/Code/UI/ShareRecordingMenuWindow.cs`

### Parse SDK for Unity
- **What it is:** Parse backend-as-a-service Unity client SDK.
- **Removed folder:** `client/Assets/Parse/`
- **Where to get:** https://github.com/parse-community/Parse-SDK-dotNET (open source, but the Unity-specific wrapper's license was unclear from what was included).
- **Note:** This project appears to have migrated away from Parse to a custom backend (`server/`). Parse may no longer be actively used.

### Unity IAP
- **What it is:** Unity's In-App Purchasing package.
- **Removed folder:** `client/Assets/IAP/`
- **Where to get:** Install via Unity Package Manager → `com.unity.purchasing`. Requires Unity Services setup.
- **Used by:** `Assets/_Assets/Code/IAP/IAPManager.cs`

### innocenttimes / RateMyGame
- **What it is:** App rating prompt dialog — shows a "Rate this app" prompt based on launches and days elapsed.
- **Removed folder:** `client/Assets/innocenttimes/`
- **Where to get:** Unknown. No license or publisher information was present. Could be replaced with any Unity rating prompt plugin.

### GifPlayer
- **What it is:** Animated GIF playback plugin for Unity UI (commercial, Chinese publisher).
- **Removed folder:** `client/Assets/Plugins/GifPlayer/`
- **Where to get:** Check Unity Asset Store. Alternatives: yasirkula/UnityGif or MG-GIF.

### SWAN Dev Plugins (ProGIF, SocialShare, Api Helpers)
- **What it is:** Suite of commercial Unity plugins — ProGIF (GIF recording/encoding), SocialShare, and API helpers.
- **Removed folder:** `client/Assets/Plugins/SWAN Dev/`
- **Where to get:** https://assetstore.unity.com/publishers/24669 (paid)
- **Used by:** `Assets/_Assets/Code/GifRecorder/GifRecorder.cs`, `Assets/_Assets/Code/UI/ShareRecordingMenuWindow.cs`

### Universal Deep Linking (Imagination Overflow)
- **What it is:** Cross-platform deep linking plugin for Unity.
- **Removed folder:** `client/Assets/Plugins/ImaginationOverflow/UniversalDeepLinking/`
- **Where to get:** https://assetstore.unity.com/packages/tools/utilities/universal-deep-linking-114074 or https://github.com/nicoplv/smart-deep-link
- **Used by:** `Assets/_Assets/Code/Framework/DeepLinkSingleton.cs`

### Unity Distribution Platform (UDP)
- **What it is:** Unity's UDP distribution service plugin.
- **Removed folder:** `client/Assets/Plugins/UDP/`
- **Where to get:** Unity Package Manager → `com.unity.purchasing.udp`. Note: UDP was shut down by Unity in 2023.

### Google Play Services Android Libraries
- **What it is:** 30+ Google Play Services and Firebase AAR libraries for Android.
- **Removed folder:** `client/Assets/Plugins/Android/`
- **Contents included:** play-services-games, play-services-auth, play-services-measurement, firebase-analytics, firebase-messaging, and supporting libraries.
- **Where to get:** These are pulled in automatically by the Firebase and GooglePlayGames Unity SDKs via the Google External Dependency Manager. Reinstalling those SDKs will restore these.

---

## Open-Source Plugins (retained)

These were kept because they have redistributable licenses:

| Plugin | Folder | License | Source |
|--------|--------|---------|--------|
| FSG iOS Keychain Plugin | `Assets/FSG/` | MIT | Unity Technologies |
| PlayerPrefsEditor | `Assets/PlayerPrefsEditor-master/` | MIT | https://github.com/sabresaurus/PlayerPrefsEditor |
| NativeGallery | `Assets/Plugins/NativeGallery/` | MIT | https://github.com/yasirkula/UnityNativeGallery |
| uGIF | `Assets/Plugins/uGIF/` | MIT | Simon Wittber |
| SQLite.cs | `Assets/SQLite/` | Public domain | https://github.com/praeclarum/sqlite-net |
| GooglePlayGames plugin | `Assets/GooglePlayGames/` | Apache 2.0 | https://github.com/playgameservices/play-games-plugin-for-unity |
| TextMesh Pro | `Assets/TextMesh Pro/` | Unity Companion License | Included with Unity |

---

## Secrets & Credentials

These were either deleted or replaced with placeholders. A new deployment will need to regenerate all of them.

### Firebase
- **Removed file:** `client/Builds/iOS/156/GoogleService-Info.plist`
- **What it contained:** Firebase API key, OAuth client IDs, GCM sender ID, project database URL.
- **How to get:** Create a Firebase project at https://console.firebase.google.com, add an iOS app with bundle ID `com.chumpware.safariforever` (or your own), and download the generated `GoogleService-Info.plist`.

### iOS Provisioning Profiles
- **Removed files:** `client/fastlane/match_AdHoc_*.mobileprovision`, `match_AppStore_*.mobileprovision`, `match_Development_*.mobileprovision`
- **How to get:** Enroll in the Apple Developer Program (https://developer.apple.com), create App ID `com.chumpware.safariforever` (or your own), and generate provisioning profiles via Xcode or Fastlane Match.

### Fastlane Credentials
- **Removed files:** `client/fastlane/.env`, `client/fastlane/.env.AppStore`, `client/fastlane/.env.SafariForever`
- **Sanitized files:** `client/fastlane/Appfile`, `client/fastlane/Matchfile` (placeholders: `YOUR_APPLE_ID`, `YOUR_TEAM_ID`, `YOUR_TEAM_NAME`, `YOUR_CERTIFICATES_REPO`)
- **How to set up:** Follow the Fastlane documentation at https://docs.fastlane.tools/getting-started/ios/setup/ with your own Apple Developer account.

### Server — Environment Variables
The server (`server/SFServer/`) reads credentials from environment variables. A `secrets.env` file (excluded from version control) should provide:

| Variable | Service | How to Obtain |
|----------|---------|---------------|
| `CONNECTION_STRING` | PostgreSQL | Your own database — see `InstallInstructions.txt` |
| `DISCORD_TOKEN` | Discord Bot | https://discord.com/developers/applications — create a bot and copy its token |
| `GIPHY_API_KEY` | Giphy GIF upload | https://developers.giphy.com — create an app |
| `GOOGLE_SERVICE_ACCOUNT` | Google Play IAP verification | Google Cloud Console — service account JSON with `androidpublisher` scope |
| `AUTHENTICATION_TOKEN_KEY` | JWT signing | Generate a random 256-bit key |
| `CLIENT_SECRET_SALT` | Client auth hashing | Generate a random salt string |
| `RSA_PRIVATE_KEY` | RSA encryption | Generate with `openssl genrsa -out private.pem 2048` |

### Server — Development Database Password
- **Sanitized in:** `server/SFServer/Startup.cs` (line ~167), `server/SFServer/InstallInstructions.txt`
- Both hardcoded local development passwords were replaced with `[REDACTED]`.
- For local dev, set any password when spinning up the Docker Postgres container and update `Startup.cs` accordingly (or switch to reading from env vars in development too).

---

## Fonts

### Server web frontend

These font files were removed from `server/SFServer/wwwroot/static/` because their redistribution licenses are unknown:

| Font | Files Removed |
|------|---------------|
| Palamecia | `palamecia.otf`, `palamecia.ttf`, `palamecia-webfont.woff`, `palamecia-webfont.woff2` |
| Superfruit | `superfruit-webfont.ttf`, `superfruit-webfont.woff`, `superfruit-webfont.woff2` |

The three HTML/Razor templates in `server/SFServer/wwwroot/Templates/` reference these fonts. Each template now has a `/* REMOVED */` comment where the `@font-face` declarations were. A replacement maintainer should:

1. Source a suitable licensed web font (e.g., Google Fonts)
2. Add the font files to `wwwroot/static/`
3. Restore the `@font-face` declarations in the three template files
4. Update font-family references in the CSS

### Unity client

These font files were removed from `client/Assets/_Assets/Font/` because they are commercial or personal-use-only:

| Font | File(s) Removed | Reason |
|------|-----------------|--------|
| Palamecia | `palamecia.otf`, `PalameciaFont.txt`, `Resources/palamecia SDF - Regular.asset`, `Resources/palamecia SDF - Outlined.asset`, `WebGLTemplates/SFTemplate/wwwroot/static/palamecia-webfont.woff`, `.woff2` | Commercial/unknown license |
| Superfruit | `Superfruit (licenced - edited).ttf`, `superfruit (licenced).otf`, `Resources/Superfruit (licenced - edited) SDF.asset`, `Resources/Superfruit (licenced - edited - Outlined) SDF.asset`, `Resources/Superfruit (licenced - edited - outlined - light)SDF.asset` | Commercial license (noted in filename) |
| Arial Rounded Bold | `Arial Rounded Bold.ttf`, `Resources/ArialRounded-Bold.asset`, `Resources/ArialRounded-Bold-Outlined.asset` | Microsoft commercial font |
| I Eat Crayons | `i eat crayons.ttf` | Freeware for personal use only, not redistributable |

Note: Unity/TextMesh Pro bakes glyph data from the source font file into `.asset` atlas files. Both the source font file and the generated atlas were removed for each commercial font.

**Fonts retained** (all under SIL Open Font License or equivalent):
- `FiraSans-Medium.ttf` — SIL OFL
- `OpenDyslexic-Regular/Bold/Italic/BoldItalic.otf` — SIL OFL
- `Rubik-Bold.ttf`, `Rubik-Medium.ttf` — SIL OFL
- `Ubuntu-Bold.ttf`, `UbuntuCondensed-Regular.ttf` — Ubuntu Font License
- `VarelaRound-Regular.ttf` — SIL OFL
- `TextMesh Pro/Fonts/LiberationSans.ttf` — SIL OFL (bundled with TextMesh Pro)

---

## CI/CD Pipeline (removed)

The build and release pipeline was Fastlane-based and has been removed entirely.
