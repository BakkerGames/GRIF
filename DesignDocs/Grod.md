# Grod design document

Grod is a memory database that stores data in string key-value pairs. It is designed to be fast and efficient, with a focus on simplicity and ease of use.

Grod stands for Game Resource Overlay Database, and it is intended to be used in game development for storing string game resources such as text, filenames, and other game-related data.

Grod uses a parent-child relationships to hold data. Each key in a parent level can be overridden by a child level. The purpose is to track all changes made to the data, allowing for easy saving and loading of game states. Reading data can be done either on the current level or by searching through all levels until a match is found.

Keys are not case-sensitive. They cannot be null or empty or only whitespace. They also cannot start or end with whitespace. Values can be null, and the reserved word `null` is used to indicate a null value.

## Get

The `Get` operation retrieves the value associated with a given key. The `recursive` parameter indicates if parent levels are searched whenever the child level does not contain a key. If the key is not found in any level, null is returned.

There are two variations of the `Get` operation: `GetNumber` and `GetBool`. `GetNumber` retrieves the value as a long. `GetBool` retrieves the value as a boolean, and other truthy/falsey values are recognized. Both return null if the key does not exist.

## Set

The `Set` operation assigns a value to a given key. If the key already exists in the current level, its value is updated. If the key does not exist, it is added to the current level. If the value is null or `null`, the key will appear to no longer exists, but stays in the database.

Values are only set in the current level. Setting a value does not affect parent levels.

## Remove

The `Remove` operation deletes a key-value pair from the current level. If the key does not exist in the current level, no action is taken.

## Clear

The `Clear` operation removes all key-value pairs from the current level. Parent levels are also cleared if the `recursive` parameter is set to true.

## Keys

The `Keys` operation returns a list of all keys in the current level. If the `recursive` parameter is set to true, keys from all parent levels are also included.

The `sorted` parameter indicates if the returned keys should be sorted alphabetically ignoring case.

`Keys` can also take a prefix key. All keys returned will start with or match that prefix.

## Items

The `Items` operation returns a list of all key-value pairs in the current level. If the `recursive` parameter is set to true, key-value pairs from parent levels are also included.

The `sorted` parameter indicates if the returned items should be sorted alphabetically by key ignoring case.

## AddItems

The `AddItems` operation adds multiple key-value pairs to the current level. If a key already exists in the current level, its value is updated. If a key does not exist, it is added to the current level.

## Usage

Grod is designed to be used in scenarios where data needs to be stored and retrieved quickly, such as in games. It can be used to store game resources, player settings, and other game-related data. It does not throw errors when updating a new value, unlike some other structures.

The parent level would be filled with default game resources, while child levels would be used to track changes made during gameplay. This allows for easy saving and loading of game states, as well as the ability to revert to default settings if needed.

Multiple Grod databases can be stacked on top of each other using the Parent property. Each has a Name property which should be filled with something useful about that level, such as the file name. This will allow game development to be done in sections, and all the sections can be stacked together to play.