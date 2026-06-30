# Safari Forever — Patch Notes

## Alpha Launch
**August 6, 2019** — Safari Forever entered alpha. Game was feature-complete; recruiting testers and working on promotional material.

**September 9, 2019** — Alpha invitations sent out. Available via Apple TestFlight and Google Play Alpha.

---

## v0.69 — Patch 1 (September 15, 2019)

**Bug fixes:**
- Player randomly jumps higher when bouncing on bullets
- Mega jump on jump pad after spawn or using portal
- Portals and Saws can be placed outside of bounds
- Jumping on a bullet right as it collides with a wall snaps the player inside the wall
- Spiketrap no longer activates before the player starts playing
- Unstable blocks don't reset correctly when verifying level
- Spikeball can spawn inside tiles
- Can enter portal even when it's blocked
- Overlapping portals teleports player all over the place
- Player can walk through walls and blocks if being teleported inside them
- Entering multiple portals at once is hilarious
- Putting portal inside conveyorbelt makes the player stuck in conveyor belt mode
- You can place slopes on the player
- Editor WIP level is not deleted when resetting game
- Possible to enter goal from above without touching the base if already grounded
- Entering goal before starting level puts the game in a weird state
- Resetting level after entering goal lets you upload a highscore of 00:00
- Readable Notifications not marked as read after visiting notification hub
- Not all notifications are clickable
- Graphical glitch when fluttering into portal
- Flag can be placed inside the bottom row outside of bounds
- Spikeball may not change direction when bouncing on jumppad
- Next Level + Load Player Level menus can be open at the same time
- Mute buttons are unreliable
- Logic switch makes it possible to clip through walls
- Non-viewable unread notifications
- "Filled" tiles make the camera zoom out further than intended

---

## v0.70 — Patch 2 (September 21, 2019)

**New:**
- Note box
- Ingame search function

**Changes:**
- Tweaked Flutter jump to make it feel more responsive
- The previous pulldown menu containing publish and clear level button has been replaced by a hamburger menu
- "Coco" has been renamed "Koko"

**Bug fixes:**
- Entering doors in player made levels does not work
- Back button not behaving as expected on Android
- Replaying levels does not work
- Possible to get stuck after completing level
- Not possible to save if placing a portal or saw exit 1 tile above, below, left or right of entrance
- Editor not loading the right soundtrack on Work in Progress being loaded
- Sensor triggerbox too large
- Possible to start running before level is revealed
- You can Like and Dislike a level at the same time
- When replaying another player's level the like counter will display one more like than the level actually has

---

## v0.71 — Patch 3 (September 23, 2019)

**Changes:**
- Spikeballs trigger sensors

**Bug fixes:**
- Possible to spawn inside Conveyor
- Possible to place items outside by opening hamburger menu and entering publish mode
- Conveyor does not always apply its effect when walking on to it from the side
- Sensor can be triggered in build mode
- Spikeball not affected by conveyor belt
- Starting and stopping a conveyor while riding it does not apply and remove the conveyor speed modifier

---

## v0.72 — Patch 4 (September 29, 2019)

**New:**
- Support for Discord daily challenges
- Another note box with an additional octave

**Changes:**
- Jump pad can now be rotated
- Cannons can now face diagonally
- Better text contrast on dialog windows
- Ambiguous characters removed from sharecodes
- Sharecodes are now 6 digits
- Added animation to Clear Search button

**Bug fixes:**
- "Alpha" expands infinitely
- You get two notifications if you beat your own record
- UI Buttons can get stuck in the pressed position
- Still possible to place items outside of level with the logic canvas
- Tapping "Home" button and "Next World" button at the same time causes multiple characters to spawn
- Getting killed by two sources at the same frame makes the level gray
- Accidentally dragging items from the toolbar for less than 0.3 seconds spawns the item on the first row of tiles
- Sharecode Level IDs contained ambiguous characters
- Dislike button no longer appears to delete likes

---

## v0.73 — Patch 5 (October 15, 2019)

**New:**
- Logic System version 2
- Hebrew language (thanks to @yot)

**Changes:**
- Spike collision boxes are a bit smaller now

**Bug fixes:**
- All cannons face upwards
- Bullets are sometimes able to fly through walls after exiting portal
- Sometimes function boxes do not reset and appear invisible (reproducible by dying right after hitting a stop box)
- Flutterjump doesn't reset when jumping on jump pad
- Dying while holding on screen locks the input in "pressed" mode
- Portals can't keep up with amount of cannonballs entering
- Overloading the animation system causes graphical glitches ("Glitch land"). Cannonball portal overload fixed.

---

## v0.74 — Patch 6 / Hotfix (October 16, 2019)

**Bug fixes:**
- ❗ Logic Gates break on loading
- Logic wires not updated when opening the logic canvas while holding Node outside of level

---

## v0.75 — Patch 7 / Hotfix (October 16, 2019)

**Bug fixes:**
- ❗ Campaign shows up as incompatible (Levels from v0.73 and up are now playable again)
- Some Hebrew fixes

---

## v0.76 — Patch 8 / Hotfix (October 19, 2019)

**New:**
- A small "thank you for being patient" surprise

**Bug fixes:**
- ❗ Fixed broken Logic System, causing some levels not to load
- Some English is rendered Right To Left when using a Right to Left Language

---

## v0.77 — Patch 9 (October 21, 2019)

**New:**
- A new Music track added to the editor
- When tapping an Output node, connected wires are highlighted

**Bug fixes:**
- Level 8-3 is not playable
- Level 9-3 crashes during loading
- Level 11-2 crashes during loading
- Changing room while placing items lets you place them outside of level
- Jumping into a function block while grounded causes the character to become unable to jump

---

## v0.78 — Patch 10 (October 27, 2019)

**New:**
- Lock n' Key
- Secret item

**Bug fixes:**
- Possible to publish levels built above the build limit
- Highscore does not show player's score if beating the WR
- Random smoke particles when starting and stopping a level
- Exiting the Idle state restarts the timer. Pause Block puts you in idle state
- Game does not suspend when viewing a level
- Game softlocks when loading a level while dying
- Bullets are not paused when game is suspended
- Possible to noclip using the spring and a stair of slopes
- Spikeball can noclip when placed inside switchblocks
- Stop Box does not send a signal
- Ingame timer sometimes shows 10ms less than the actual player time
- Game softlocks while loading a level (iOS only?)
- Logic canvas delete thickness varies based on angle
- Menu does not close when loading a sharecode

---

## v0.79 — Patch 11 (November 9, 2019)

**New:**
- Laser cannon
- Cloud saving for your account and progress (existing users can enable from settings)
- Autosaving in the editor (saves every time you enter test mode)

**Improvements:**
- 33% smaller install size
- 35% less memory usage

**Bug fixes:**
- Highscore shows up as 00:00 in summary
- "Sex" in the dictionary
- Power Supply lightning bolt does not return to gray when unpowered
- Power Supply changes state one time too many when first entering level
- Spike trap can kill a player in edit mode
- Logic canvas does not scale correctly by aspect ratio
- Portal graphic is closed, but can still be entered after resetting level
- Spikeballs do not always respawn
- Logic System not behaving consistently when level is first loaded and when reset
- Race condition causing bullets to be destroyed before breakable blocks are broken

---

## v0.81 — Patch 12 (November 13, 2019)

**Changes:**
- Autosaving has been disabled

**Bug fixes:**
- Some Hebrew dialogs are displayed left to right instead of right to left
- Race condition sometimes causing previously given likes not to be displayed
- Lag when you press the play button
- Lasers do not recalculate length when obstructions are enabled/disabled
- Laser is stopped by one way platformers
- Using too many lasers causes the game animation system to crash
- One way platform does not work right with bullets

---

## v0.82 — Patch 13 (November 19, 2019)

**Changes:**
- Updated credits
- Level 7-3 has been made slightly easier

**Bug fixes:**
- Lasers don't kill on the edge of the laser
- If level has no highscore, yours won't be yellow
- Conveyorbelts struggle to know what side the player is on
- If player enters a portal while on a conveyor belt their conveyor belt effect will be permanent
- Spikes can kill player in editor mode
- Using a conveyor belt to slide up into a switch box creates some weird behavior
- The amount you have to scroll to view the next page of notifications increases for each attempt
- Game won't load when opening levels while game is not running on iOS
- You can press the clear floor button and when you start the level a prompt will ask if you're sure — while the level has already started
- Tip box not displaying text
- *(Won't fix)* Possible to use slopes to break one way gates

---

## v0.83 — Patch 14 (November 30, 2019)

**New:**
- See all levels made by another player including your own

**Changes:**
- Improved UI
- Some font adjustments

**Bug fixes:**
- On some levels the punch hole transition does not fully open
- Resource meter missing button text
- Android users are not able to sign in to Google Play Games
- Android: If player deletes cloud save from Google Play Games App, app does not show enable cloud saving button
- Laser sound continues even after leaving level
- Portal does not teleport player on entering play mode

---

## v0.84 — Patch 15 (December 6, 2019)

**New:**
- Color picker lets you choose the color of your levels and your profile

**Bug fixes:**
- Opening Level Inspector by URL while viewing another window creates overlapping UI
- Portals and cannon don't play nice when positioned right next to each other
- Spikeball gets stuck on jumppad pointing in the wrong direction
- Notification count does not update after reading notifications
- Name on main menu does not update after changing nickname

---

## v0.85 — Patch 16 (December 19, 2019)

**New:**
- You can now follow other players
- Corner spikes
- Bullets vanish when jumped on
- Level 2 now has an extra room

**Bug fixes:**
- Restart level button does not work
- Campaign Level Stat popup has overlapping text
- Corner spikes may overlap
- Profile color button visible when viewing other player's profile
- Some missing translations

---

## v0.86 — Patch 17 (January 3, 2020)

**New:**
- Push Notification support
- Level Browser:
  - Browse by New
  - Browse by Trending (levels with most plays recently)
  - Browse by Popular (levels ranked by most likes overall)
  - Alpha/Beta badge displayed next to your name on your profile
  - Daily challenge available ingame
  - Level of the week available ingame

**Improvements:**
- Color picker hue wheel now changes based on selected vibrance and saturation
- New graphic for sensor
- New graphic for delay

**Bug fixes:**
- Timer sometimes stops at 99.98
- Number of followers doesn't update when following or unfollowing without refreshing
- If bullet collides on exiting portal, animation system will crash
- Wallpaper button label with wallpaper #10 is no longer split across two lines
- Spiketrap can kill player in editor
- Oscillator sends out power on first frame
- Restored profiles did not successfully retrieve their color from server
- Logic System does not behave consistently when playing the level for the first time in the editor and when later loading that level
- App does not boot on first attempt on iOS after installing app
- Profile button does not appear before reloading main menu after registering / restoring
- Game does not launch without internet — and doesn't explain why
- Level starts while screen transition is playing
- Connecting OR gates in both directions does not behave as expected
- Stacking Delay on top of power supply created a weird graphical glitch

---

## v0.87 — Patch 18 (January 19, 2020)

**New:**
- 6 more levels in the campaign
- New soundtrack for your levels
- Notification toggles

**Improvements:**
- Game returns you to what you were looking at before loading a level
- Smoother loading of notifications and levels when scrolling
- Oscillator has an input — can be toggled on and off with power supply
- Notifications referring to levels will load them when clicked

**Changes:**
- Spikeball now loses its momentum when entering a Portal
- Sensor renders behind portal (and a lot of other stuff)
- One way gate renders further in the background (and a lot of other stuff)

**Bug fixes:**
- Pause box does not send a signal on being used
- Request your data results in a 500 error
- Lasers take longer to fire the first time when entering a level
- After browsing a user's level from their profile, profile will become the same color as the level
- Power supply does not emit power on the first frame when entering a room
- Breakable blocks can be broken from the sides
- The top part of a left facing slope is reversed
- If you use room slot 1 & 4 but leave 2 & 3 blank when publishing you will get "\<level name\> - 4" after going through the door
- Key can be picked up in edit mode
- One way gate may remain open in edit mode after exiting test mode
- Laser sounds remain after a laser has stopped firing

---

## v0.88 — Patch 19 (February 1, 2020)

**New:**
- Banana rewards displayed ingame
- Latch: Like a power supply but once toggled will remain in that position for the rest of the room
- Like/dislike levels without beating them first

**Improvements:**
- New notification type: Get notified when someone you follow publishes a level

**Bug fixes:**
- Loading the last level in the browser causes the window to softlock
- Trending levels repeat the same levels
- Trending levels not sorted in the correct order
- Exiting level entered from the browser menu does not load featured levels when returning
- Glitchland possible when chaining portals with cannons
- Sometimes when entering goal the player would start sliding upwards

---

## v0.89 — Patch 20 (February 4, 2020)

**New:**
- See what levels you played in the browser

**Bug fixes:**
- Player levels would be loaded in the editor after playing them

---

## v0.90 — Patch 21 (February 11, 2020)

**New:**
- New search feature — search for players, level names and level IDs
- Player profiles are now shareable in the same way as levels
- 2 new levels

**Improvements:**
- Better Like/Dislike icons

**Changes:**
- Latch icon color reversed

**Bug fixes:**
- Not all items block goal
- Sharp slopes behave weird when jumping on their back
- Daily challenge level beaten overlaps with leading player
- Sometimes when loading a level game will softlock and display a solid color
- Game can be softlocked by pausing while a transition is playing
- Last active is not accurate
- Restore does not update menu
- The level's creator is unable to compete in the daily challenge

---

## v0.91 — Patch 22 (March 29, 2020)

**New:**
- Adoptable characters — finally something to spend bananas on
- Two new characters to adopt
- World 2 Complete!
- Record sexy GIFs while playing (requires device with 2.5 GB RAM and at least 4 CPU threads)
- Swipe down to restart room — no more hand cramps
- Bubbles!
- Smoother UI
- Work in progress is now saved when clicking the burger menu in the editor
- Better error handling under the hood

**Changes:**
- The 4 second bullet lifetime has been removed
- No more than 50 bullets can be on screen at the same time
- Saws now have smaller hitbox
- Spikes have smaller hitbox
- Restart button removed
- Some words removed from dictionary

**Bug fixes:**
- Banana notifications not being received
- Bullets disappear after 4 seconds
- "I am Key. Destroyer of worlds."
- Campaign highscores can be reset after changing room
- Local highscores are sometimes lost
- When entering level editor portal line would show despite basic tab not being selected
- SFX plays an infinite amount of sounds when connected to a power source
- Color, Wallpaper and Floor buttons don't update when loading a level
- Spikeball starts rolling when entering editor pause menu
- Resetting while death animation is playing puts the game in slow mode
- You can receive multiple rewards from the same daily challenge
- Missing Hebrew text in some dialog buttons (wrong font)
- Scrolling through notifications breaks after loading more by scrolling
- Loading users by their share URL showed your levels on their profile page

---

## v0.92 — Patch 23 (March 31, 2020)

**Bug fixes:**
- Flutterjump is not reset when exiting a bubble
- Bubbles make the player fly about if entering multiple bubbles at the same frame
- Putting the Player on top of Goal in the editor, hitting the play button and then reloading softlocks the level
- Items that trigger their effect when player touches them don't work the first time a level is played if player spawns on top of them
- Portals trigger before the player has started running
- Falling blocks trigger in the editor

---

## v0.93 — Patch 24 (April 20, 2020)

**New:**
- In-app purchases — characters can now be bought
- Change consent from settings for players in the EU and EEA
- Facebook integration for recruiting new players using Facebook ads
- Proguard on Android to make it slightly less convenient to peek at the code

**Changes:**
- Spike trap replaced with table saw
- Spikes replaced with "Short Spikes" that can't be jumped or walked on
- Firebar now also has a flame in the center, making it better suited for airborne traps
- Campaign levels updated to use new short spikes and table saw
- Prettier dialog boxes
- Better looking toggle switches in the notification option menu

**Bug fixes:**
- iPad Pro opens WebGL version instead of game
- Giphy links expire
- Loading the last message in inbox softlocks the list
- Added missing Norwegian characters to Rubik font

---

## v0.94 — Patch 25 (April 22, 2020)

**New:**
- Notifications now support URL links

**Bug fixes:**
- Missing text on some buttons when using Hebrew (like the color picker)
- Short spikes make it impossible to publish a level
- Bananas are square
- Fixed mirrored LTR text

---

## v0.97 — Patch 26 (May 5, 2020)

**New:**
- Possible to show previous daily challenges and weekly levels

**Improvements:**
- Easily view your levels in the boosted list
- Improved World 1 Level 10

**Changes:**
- Less bounce on bullets

**Bug fixes:**
- Can't load more than 25 levels and notifications in the browser
- Players are not receiving their first reward
- Share button does not work on Android
- Bullets don't care about game speed
- Pendulum doesn't care about game speed
- Oscillator is not emitting the "off" signal when not powered, causing connected objects to become unresponsive to other power sources
- Connecting a power source to cannon fires wildly

---

## v0.98 — Patch 27 (May 6, 2020)

**Bug fixes:**
- Physics changed based on game speed

---

## v0.99 — Patch 28 (May 12, 2020)

**New:**
- Metal block

**Bug fixes:**
- After beating a level going to the creator profile and back again shows the like/dislike buttons again

---

## v0.100 — Patch 29 (June 6, 2020)

**Changes & Improvements:**
- Cannon upgrade: More granular control over aiming
- Laser upgrade: More granular control over aiming
- Auto targeting cannons
- Auto targeting lasers
- Firebar upgrade: More granular control over start rotation
- Sensor upgrade: You can change its size
- Sensor now emits continuous power instead of a pulse
- Spikes have been tuned so you can walk on their back and they no longer kill you when sliding past them
- Latch is now called "Fuse"

---

## v1.01 — Patch 30 (June 9, 2020)

**Bug fixes:**
- Boosting button loads half opacity white screen instead of boosting menu
- Metal block has no collision when electric
- Game does not return you to the correct menu after playing a level
- Sensor is gray after loading a level

---

## v1.02 — Patch 31 (June 14, 2020)

**Improvements:**
- Level browser should become much faster in a few weeks
- Some audio design improvements to Goal fanfare and Death effect
- More death scream variations
- New sounds for Cannon
- Higher quality sounds for Bubbles

**Bug fixes:**
- Electric block doesn't always kill you when you walk up to it
- Can't flutterjump after hitting a pause block
- Exiting a level after hitting the "more" button exits to the main menu instead of where you entered the level
- Editor shortcuts work on Android when using a keyboard

---

## Release
**June 18, 2020** — Safari Forever is now publicly available on the App Store and Google Play!

---

## v1.1 — Patch 32 (June 27, 2020)

**New:**
- Swedish language
- Vietnamese language
- Italian language
- Portuguese (BR) language
- Latin American Spanish language

**Improvements:**
- Select country now has a search box
- Language is now selected from a list
- No more rerolling level names — Adjective + Noun can now be picked from a list
- GIFs now have your nickname in the corner

**Changes:**
- Pendulum has a slightly smaller hitbox
- Items that require continuous power now require two frames of power before they activate
- Portals now reset flutter jump on exit

**Bug fixes:**
- Hong Kong is missing from the countries list
- Resetting the level while watching the win screen/death animation does not reset the music volume
- Uploading your first level without having accepted the Terms of Service softlocks the game
- Flutter jump brings the player too high after exiting a portal
- One way platforms placed below spikes mess with the spikes' collision detection
- Tapping too fast in the level browser can cause multiple menus to be open at the same time
- Bullets are sometimes destroyed by one way gates on their way through
- Bubbles don't suspend when game is paused
- Items that move with a single "tick" of power behave inconsistently between devices
- You die when running over an edge with spikes on the cliff edge

---

## v1.2 — Patch 33 (July 10, 2020)

**New:**
- Finnish language
- Turkish language

**Improvements:**
- New (and more expensive) font
- Reworked UI to better support multiple languages
- Select Language is now localised
- Stopwatch goes above 99:99
- Share button added to Level Menu
- Level editor asks if you are sure before deleting your work in progress

**Bug fixes:**
- World 2 title is missing
- Reward Screen is not using the localised font for its description text
- Your personal best is incorrectly rounded down in the menu UI
- Sometimes world record does not update in the campaign
- "You already unlocked this character" appears randomly after having bought a character using IAP
- Nickname not visible on GIFs
- Table saw may trigger after the level has been reset
- Possible to have -1 notifications
- Unable to browse levels after returning to the browser after receiving a reward
- Items placed near the bottom of the screen are hard to touch/move when the entire level space is being used
- Spikeball can kill you while it is teleporting
- Can't open profile from notification
- Boosting with more bananas than you have throws a server error
- One way platform pointing down makes you fall through one way platforms
- There is no timeout feature when internet cuts during loading
- One-ways that point to the wall can make you exit bounds
- Some items have their timing wrong in slow mode (Laser, Cannon, Bubble, FunctionBox, Goal, OneWayGate, Portals, Table saw)
- Share menu not working in iPadOS 13.5.1
- Conveyor belt speed is too slow in slow mode

---

## v1.3 — Patch 34 (August 3, 2020)

**New:**
- Save your work in progress and work on multiple levels at the same time
- Horizontal mode for screens larger than 9.7"
- Better level analytics to determine difficulty
- Jelly block — wedge yourself in!
- New character: Brine!
- Estonian language

**Improvements:**
- Spikeball can now be stacked
- Bouncing on bullets now resets your flutter jump
- 3 new Saw sizes
- 0.25 option added to Delay
- 0.25 option added to Oscillator

**Bug fixes:**
- Bubble path moved post-level published creating an unbeatable level
- `<ltr>` tags visible when loading level in Left-to-Right language
- Giphy lets you skip floors
- Still possible to have -1 notifications
- You can jump from the corner of electric blocks without dying
- Restarting after dying sometimes causes a weird gray fade
- Sometimes color palette button in editor does not match level colors after publishing
- Spikeball hitbox is temporarily enlarged by portals after exiting
- Entering a portal while in a bubble makes you spin while exiting the portal
- Entering goal while entering a bubble creates a weird animation
- Possible to trigger door/flag and still die
- Putting a portal on top of a flag with bounce pad created some weird behavior
- Possible to have -1 followers
- Levels with a 00:00 time are not marked as complete
- Unfollowing user does not work
- Conveyor belt sometimes slides the player in the wrong direction
- Breakable blocks sometimes don't break
- Character selection softlocks the game
- Buying characters on Android is broken
- Pause block allows you to mega jump
- Receiving a notification while playing opens the level preview
- Stopwatch goes bananas if level preview is opened during gameplay
- Flutterjump does not get canceled if letting go of the screen while swiping down
- Aiming cannons don't aim at the snail

---

## v1.4 — Patch 35 (August 29, 2020)

**New:**
- Accessibility option to disable bright flashes
- See who currently has the World Record on any level

**Improvements:**
- Smoother transitions between levels and menu
- Jump pads can only send a signal every 3 frames
- Improved level statistics
- Invisible backend improvements

**Bug fixes:**
- GIF recording does not work right in horizontal mode
- Possible to enter goal more than once
- Possible to get stuck inside logic block and move outside of bounds
- Spikeball can get stuck inside logic block and move outside of bounds
- Search does not work
- Weird animation when entering bubble while stuck in slime
- Stacked spikeballs behave weird in portals
- Tilting your phone closes the level editor toolbar (iOS workaround: lock screen orientation)
- When attempting to go back to a creator's profile after playing one of their levels you sometimes get a 404 error
- After playing one of your own levels, you are unable to go back to creator profile
- Saw size 3 does not always return
- Back button does not always return you to the menu you came from
- Highscore rounding error
- List of available names in Level Name picker gets shorter every time you open it
- Cannons powered by jump pads stop firing on devices with low framerate

---

## v1.5 — Patch 36 (September 18, 2020)

**Changes:**
- Removed IAP purchases — the game is now 110% free

**Bug fixes:**
- Sometimes possible to publish the same level multiple times
- Missing icon for "No music" in level editor
- Fixed slowdown in Editor after exiting playmode when dying and bright flashes are disabled
- Speedy not tracked by aiming cannons (again)
- Bullets move slower on faster devices

---

## v1.7 — Patch 37 (October 1, 2020)

**New:**
- Endless Mode!

**Improvements:**
- Updated some translations

**Bug fixes:**
- Spikeballs sometimes stop rolling after exiting a portal

---

## v1.8 — Patch 38 (October 10, 2020)

**Improvements:**
- Updated some translations

**Bug fixes:**
- Logic block is a little too eager to crush player when hitboxes are touching but not intersecting
- Game crashes if spam tapping while a level is loading
- Your endless rank is displayed on other players' profiles
- If you played all levels in endless mode, you no longer have access to leaderboard

---

## v1.9 — Patch 39 (January 5, 2021)

**New:**
- Bet bananas and win hats!
- ~~New Bugs~~

---

## v1.10 — Patch 40 (January 19, 2021)

**Changes:**
- Slot machine has been replaced with a card game

**Bug fixes:**
- Random hat button now works

---

## Sunsetting (June 27, 2026)

Safari Forever has been sunset. Two things happened at roughly the same time:

1. **Google removed the game** due to a security issue in the older version of Unity (2017) being used. While the vulnerability didn't affect Safari Forever, the rules required a fix. Upgrading from Unity 2017 to a modern standard requires stepping through seven different versions, fixing broken code at every step. Many third-party plugins are completely dead and abandoned, meaning they'd have to be rebuilt from scratch. Apple and Google also now have stricter compliance, API, and privacy requirements that would require a total modern overhaul.

2. **The online server completely died** around the same time, refusing to boot back up.

Given the massive amount of unpaid time and effort required to fix these issues, combined with $100/month hosting costs and a low active player count, the difficult decision was made to sunset the game.

*Thank you to everyone who played and supported Safari Forever over the years.*
