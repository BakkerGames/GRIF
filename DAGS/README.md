# DAGS - Data Access Game Scripts

DAGS is a simple scripting engine which directly connects to a key/value dictionary. It has commands to get and set data and manipulate it in various ways. It is interpretive and not compiled in order to give it greater flexibility.

DAGS consists of functions starting with "@" with zero or more parameters defined for each. Parameter values can be strings or other functions returning strings. Strings are anything not starting with "@", and don't need quotes around them if they have no spaces or special symbols. Some functions, such as arithmetic functions, expect the string parameters to be numbers or functions returning numbers.

Many of the built-in functions directly access the data from the key/value dictionary. "@get(mykey)" reads the key "mykey" and returns the value. "@set(mykey,myvalue)" sets mykey=myvalue in the dictionary. Other functions return text to the calling program or execute more scripts.

DAGS can be extended by creating new functions and adding them to the dictionary. They are called just like the built-in functions and are usable for any purpose.

DAGS works great with GROD, a Game Resource Overlay Dictionary. GROD holds base values plus all changes in an overlay as the game is played, so changes can be saved and restored. See the [GROD](https://github.com/BakkerGames/GROD) GitHub site for more details.