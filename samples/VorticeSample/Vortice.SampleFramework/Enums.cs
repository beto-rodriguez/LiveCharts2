// Copyright (c) Amer Koleci and contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// https://github.com/amerkoleci/Vortice.Windows

namespace Vortice;

public enum KeyboardKey
{
    None,
    Cancel, // Control-break processing
    Backspace, // Backspace key
    Tab, // Tab key
    Clear, // Clear key
    Enter, // Enter key
    Pause, // Pause key
    CapsLock, // Caps Lock key
    Kana, // Japanese kana key
    Hanja, // Japanese Hanja key
    Kanji, // Japanese Kanji key
    Yen, // Japanese Yen key
    Ro, // Japanese Ro key
    katakana, // Japanese Katakana key
    hiragana, // Japanese Hiragana key
    katakanaHiragana, // Japanese Katakana / Hiragana key
    henkan, // Japanese Henkan key
    muhenkan, // Japanese non-conversion key
    numpadJpcomma, // Japanese numeric keypad comma key
    Escape, // Esc key
    Space, // Spacebar
    PageUp, // Page Up key
    PageDown, // Page Down key
    End, // End key
    Home, // Home key
    Left, // Left Arrow key
    Up, // Up Arrow key
    Right, // Right Arrow key
    Down, // Down Arrow key
    Select, // Select key
    Print, // Print key
    Execute, // Execute key
    PrintScreen, // Print Screen key
    Insert, // Ins key
    Del, // Delete key
    Equal, // Equal key
    Help, // Help key
    Num0, // 0 key
    Num1, // 1 key
    Num2, // 2 key
    Num3, // 3 key
    Num4, // 4 key
    Num5, // 5 key
    Num6, // 6 key
    Num7, // 7 key
    Num8, // 8 key
    Num9, // 9 key
    a, // A key
    b, // B key
    c, // C key
    d, // D key
    e, // E key
    f, // F key
    g, // G key
    h, // H key
    i, // I key
    j, // J key
    k, // K key
    l, // L key
    m, // M key
    n, // N key
    O, // O key
    P, // P key
    Q, // Q key
    R, // R key
    S, // S key
    T, // T key
    U, // U key
    V, // V key
    W, // W key
    X, // X key
    Y, // Y key
    Z, // Z key
    LeftSuper, // Left Windows key (Microsoft Natural keyboard) on Windows, left Command key on macOs
    RightSuper, // Right Windows key (Natural keyboard) on Windows, right Command key on macOs
    Menu, // Applications key (Natural keyboard)
    Sleep, // Computer sleep key
    Power, // Computer power key
    NumPad0, // Numeric keypad 0 key
    NumPad1, // Numeric keypad 1 key
    NumPad2, // Numeric keypad 2 key
    NumPad3, // Numeric keypad 3 key
    NumPad4, // Numeric keypad 4 key
    NumPad5, // Numeric keypad 5 key
    NumPad6, // Numeric keypad 6 key
    NumPad7, // Numeric keypad 7 key
    NumPad8, // Numeric keypad 8 key
    NumPad9, // Numeric keypad 9 key
    NumPadMultiply, // Numeric keypad multiply key
    NumPadPlus, // Numeric keypad plus key
    NumPadSeparator, // Numeric keypad comma key
    NumPadMinus, // Numeric keypad minus key
    NumPadDecimal, // Numeric keypad decimal key
    NumPadDivide, // Numeric keypad divide key
    NumPadEnter, // Numeric keypad enter key
    NumPadEqual, // Numeric keypad comma key
    NumPadPlusminus, // Numeric keypad plus/minus key
    NumPadLeftParen, // Numeric keypad comma key
    NumPadRightParen, // Numeric keypad comma key
    F1, // F1 key
    F2, // F2 key
    F3, // F3 key
    F4, // F4 key
    F5, // F5 key
    F6, // F6 key
    F7, // F7 key
    F8, // F8 key
    F9, // F9 key
    F10, // F10 key
    F11, // F11 key
    F12, // F12 key
    F13, // F13 key
    F14, // F14 key
    F15, // F15 key
    F16, // F16 key
    F17, // F17 key
    F18, // F18 key
    F19, // F19 key
    F20, // F20 key
    F21, // F21 key
    F22, // F22 key
    F23, // F23 key
    F24, // F24 key
    numLock, // Num Lock key
    scrollLock, // Scroll Lock key
    leftShift, // Left Shift key
    rightShift, // Right Shift key
    leftControl, // Left Control key
    rightControl, // Right Control key
    leftAlt, // Left alt key
    rightAlt, // Right alt key
    modeChange, // Mode change key
    semicolon, // for US ";:"
    Plus, // Plus Key "+"
    comma, // Comma Key ","
    Minus, // Minus Key "-"
    period, // Period Key "."
    slash, // for US "/?"
    grave, // for US "`~"
    LeftBracket, // for US "[{"
    Backslash, // for US "\|"
    RightBracket, // for US "]}"
    Quote, // for US "'""
    oemAx, // for Japan "AX"
    intlBackslash, // "<>" or "\|"
    crsel, // CrSel key
    exsel, // ExSel key
    app1, // Launch application 1 key
    app2, // Launch application 2 key
    Back, // AC back key
    forward, // AC forward key
    refresh, // AC refresh key
    stop, // AC stop key
    search, // AC search key
    bookmarks, // AC bookmarks key
    homepage, // AC home key
    mute, // Audio mute button
    volumeDown, // Audio volume down button
    volumeUp, // Audio volume up button
    audioPlay, // Play/pause media key
    audioStop, // Stop media key
    audioNext, // Play next media key
    audioPrevious, // Play previous media key
    mediaSelect, // Launch media select key
    mail, // Mail function key

    Count
}
