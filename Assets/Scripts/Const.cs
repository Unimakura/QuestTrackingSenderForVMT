﻿public static class PlayerPrefsKey
{
    public const string IP = "KEY_IP";
    public const string PORT = "KEY_PORT";
    public const string FPS_INDEX = "KEY_FPS_INDEX";
    public const string ADJUST_ABNORMAL_POSITION = "KEY_ADJUST_ABNORMAL_POSITION";
    public const string SMOOTH = "KEY_SMOOTH";
}

public static class DefaultValue
{
    public const string IP = "192.168.0.1";
    public const string PORT = "39570";
    public const int FPS_INDEX = 0;
    public const int ADJUST_ABNORMAL_POSITION = 1;
    public const int SMOOTH = 1;
}

public static class SendTrackerValue
{
    public const int  THRESHOLD_LOCK_POS = 10; // ロック判定
}

public static class Label
{
    public const string START = "START";
    public const string SENDING = "NOW SENDING";
}

public static class TrackerIndex
{
    public const int HIP = 0;
    public const int LEFT_LEG = 1;
    public const int RIGHT_LEG = 2;
}