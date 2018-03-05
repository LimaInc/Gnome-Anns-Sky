# Gnome Ann's Sky

## Easy method to run on 64-bit linux
* Run ./run.sh in the project root.
* The correct Mono version, as described below is still required.
* If this doesn't work, you must run it through the Godot editor, as described below.

## Building
* Install [Godot](https://godotengine.org/download)
    * Mono version (C# Support) is required.
* Install [Mono](http://www.mono-project.com/download/stable/)
    * As of 15/02/18, there is a bug in Godot, which means you must install **Mono 5.4**, not the latest version which is 5.8.
        * On OS X and Windows, older versions can be found in the [archive](https://download.mono-project.com/archive/).
        * For Linux, follow the instructions [here](http://www.mono-project.com/docs/getting-started/install/linux/#accessing-older-releases), replacing "wheezy", with the name of the mono version you want (e.g. xenial for Ubuntu 16.04).
* Install [Git](https://git-scm.com/downloads)
    * For Linux, the easiest way is via the command line with "sudo apt install git"
    * For OS X and Windows, there are installers on the git site.
        * You may instead want to use a GUI client such as [GitHub Desktop](https://desktop.github.com/).
* Clone the [source repository](https://github.com/LimaInc/SustainableCraft) from Github
    * On the command line, enter:
    ````
    git clone https://github.com/LimaInc/SustainableCraft.git
    ````
    * Note this will clone the project into your current working directory, so put it somewhere sensible.
    * If you are using a GitHub Desktop, the clone button on the repository page should open your client and clone the repository.
* Start up Godot
    * Run the Godot_v3.0-stable_mono_etc file which you downloaded
* Open the project in Godot
    * Press import on the right hand panel in Godot.
    * Click Browse and navigate to the folder in which you cloned the repository.
    * In the root of the repository, there will be a file named "project.godot" - open this.
* Run the game
    * You should now be able to run the game by pressing the play button in the top right of the Godot editor window, or with the keyboard shortcut F5.

## Additional Information
Using Godot version v3.0.stable.official.mono

If you are working on Godot stuff, create a new scene. Merging scene files is quite a hassle
