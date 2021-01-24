﻿public static class PlayerPrefsKey
{
    public const string IP = "KEY_IP";
    public const string PORT = "KEY_PORT";
    public const string FPS_INDEX = "KEY_FPS_INDEX";
    public const string ADJUST_ABNORMAL_POSITION = "KEY_ADJUST_ABNORMAL_POSITION";
    public const string SMOOTH = "KEY_SMOOTH";
    public const string THRESHOLD_MOVE_POS = "KEY_THRESHOLD_MOVE_POS";
}

public static class DefaultValue
{
    public const string IP = "192.168.0.1";
    public const string PORT = "39570";
    public const int FPS_INDEX = 0;
    public const int ADJUST_ABNORMAL_POSITION = 0;
    public const int SMOOTH = 1;
    public const float THRESHOLD_MOVE_POS = 5f; // 5m/s

}

public static class SendTrackerValue
{
    public const int  MAX_ADJUST_ABNORMAL_POS = 3; // 異常値調整回数
    public const int  SKIP_ADJUST_ABNORMAL_POS = 3; // 異常値調整をスキップする回数（異常値調整が止まらなくなってしまうのを防ぐ為）
    public const float LERP_RATE = 0.5f;
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