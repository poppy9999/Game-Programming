using System;
using HideAndSeek;
using Mirror;
using UnityEngine;

public static class MySerializer
{
    public static void WriteGuid(this NetworkWriter writer, Guid value)
    {
        writer.WriteString(value.ToString());
    }
    public static void WriteGameType(this NetworkWriter writer, GameType value)
    {
        writer.WriteByte((byte)value);
    }
    public static Guid ReadGuid(this NetworkReader reader)
    {
        return Guid.Parse(reader.ReadString());
    }
    public static GameType ReadGameType(this NetworkReader reader)
    {
        return (GameType)reader.ReadByte();
    }
}