using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;

[StructLayout(LayoutKind.Explicit, Size = 7)]
public struct LogitechDualActionHIDInputReport : IInputStateTypeInfo
{
    public FourCC format => new FourCC('H', 'I', 'D');

    [FieldOffset(0)] public byte garbageByte;

    [InputControl(name = "leftStick", layout = "Stick", format = "VEC2", sizeInBits=16)]
    [InputControl(name = "leftStick/x", offset = 0, format = "BIT", sizeInBits=8, parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/left", offset = 0, format = "BIT", sizeInBits=8, parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "leftStick/right", offset = 0, format = "BIT", sizeInBits=8, parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    [InputControl(name = "leftStick/y", offset = 1, format = "BIT", sizeInBits=8, parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "leftStick/up", offset = 1, format = "BIT", sizeInBits=8, parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "leftStick/down", offset = 1, format = "BIT", sizeInBits=8, parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1,invert=false")]
    [FieldOffset(0)] public byte leftStickX;
    [FieldOffset(1)] public byte leftStickY;

    [InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
    [InputControl(name = "rightStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1")]
    [InputControl(name = "rightStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "rightStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0,clampMax=0.5,invert")]
    [InputControl(name = "rightStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp,clampMin=0.5,clampMax=1,invert=false")]
    [FieldOffset(2)] public byte rightStickX;
    [FieldOffset(3)] public byte rightStickY;

    [InputControl(name = "dpad", format = "BIT", layout = "Dpad", sizeInBits = 4, defaultState = 8)]
    [InputControl(name = "dpad/x", offset = 0, format = "BIT", layout="DpadAxis", sizeInBits=4, parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "dpad/left", offset = 0, format = "BIT", layout="DiscreteButton", sizeInBits=4, bit=0, parameters = "minValue=5, maxValue=7")]
    [InputControl(name = "dpad/right", offset = 0, format = "BIT", layout="DiscreteButton", sizeInBits=4, bit=0, parameters = "minValue=1,maxValue=3")]
    [InputControl(name = "dpad/y", offset = 0, format = "BIT", layout="DpadAxis", sizeInBits=4, parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
    [InputControl(name = "dpad/up", offset = 0, format = "BIT", layout="DiscreteButton", sizeInBits=4, bit=0, parameters = "minValue=7,maxValue=1,nullValue=8,wrapAtValue=7")]
    [InputControl(name = "dpad/down", offset = 0, format = "BIT", layout="DiscreteButton", sizeInBits=4, bit=0, parameters = "minValue=3,maxValue=5")]
    [InputControl(name = "buttonWest", displayName = "One", bit = 4)]
    [InputControl(name = "buttonSouth", displayName = "Two", bit = 5)]
    [InputControl(name = "buttonEast", displayName = "Three", bit = 6)]
    [InputControl(name = "buttonNorth", displayName = "Four", bit = 7)]
    [FieldOffset(4)] public byte buttons1;

    [InputControl(name = "leftShoulder", displayName = "Five", bit = 0)]
    [InputControl(name = "rightShoulder", displayName = "Six", bit = 1)]
    [InputControl(name = "leftTriggerButton", displayName = "Seven", layout = "Button", bit = 2)]
    [InputControl(name = "rightTriggerButton", displayName = "Eight", layout = "Button", bit = 3)]
    [InputControl(name = "leftTrigger", displayName = "Seven", layout = "Button", bit = 2, format = "BIT")]
    [InputControl(name = "rightTrigger", displayName = "Eight", layout = "Button", bit = 3, format = "BIT")]
    [InputControl(name = "select", displayName = "Nine", bit = 4)]
    [InputControl(name = "start", displayName = "Ten", bit = 5)]
    [InputControl(name = "leftStickPress", bit = 6)]
    [InputControl(name = "rightStickPress", bit = 7)]
    [FieldOffset(5)] public byte buttons2;
}
