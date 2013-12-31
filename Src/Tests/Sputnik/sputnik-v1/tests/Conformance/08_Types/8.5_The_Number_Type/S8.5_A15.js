/**
 * @name: S8.5_A8;
 * @section: 8.5, 7.8.3;
 * @assertion: Number.toString(16) should return valid number
 * @description: https://github.com/fholm/IronJS/issues/102
*/

var n = 9999;
var n16 = n.toString(16)

if (n16 !== "270F") {
    $ERROR("#1: Number.toString(16) should return valid number. Expected 270F, but have " + n16);
}

n = -9999;
n16 = n.toString(16)

if (n16 !== "-270F") {
    $ERROR("#2: Number.toString(16) should return valid number. Expected -270F, but have " + n16);
}