using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.VisualScripting;
using System.Reflection;
using System.Linq;

public static class GameSettings
{
    [Expose] public static bool IsLeftClient = false;
    [Expose] public static bool IsRightClient = false;

    #region Public Methods
    public static void WriteFile() => _WriteFile();
    public static void ReadFile() => _ReadFile();
    public static void DebugAllVars() => _DebugAllVars();
    #endregion

    #region NONPUBLIC

    static string dirPath = Application.streamingAssetsPath + "/Settings/";
    static string path = dirPath + "Settings.txt";
    static List<ExposedVar> exposedVars = new List<ExposedVar>();
    static bool hasFile = false;
    static readonly FieldInfo[] infos = typeof(GameSettings).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

    [RuntimeInitializeOnLoadMethod]
    static void OnStart()
    {
        Declare();
        MakeDirectory();
        MakeFile();
        _ReadFile();
        //_WriteFile();
#if DEBUG
        _DebugAllVars();
#endif
    }

    static void _DebugAllVars()
    {
        foreach(FieldInfo info in infos)
        {
            List<Attribute> exposed = info.GetAttributes(typeof(Expose)).ToList();
            if (exposed.Count == 0) continue;
            Debug.Log(info.Name + " : " + info.FieldType.Name + " : " + info.GetValue(null));
        }
    }

    static void _WriteFile()
    {
        List<string> vars = new List<string>();
        foreach(ExposedVar var in exposedVars)
        {
            if (var.varType == VarType.NONSUPPORTED) continue;
            vars.Add(MakeString(var));
        }

        try
        {
            File.WriteAllLines(path, vars.ToArray());
        }
        catch (Exception e) { Debug.LogError(e); }
    }

    static void _ReadFile()
    {
        if (!hasFile) return;
        try
        {
            string[] lines = File.ReadAllLines(path);
            try
            {
                foreach(string line in lines)
                {
                    try
                    {
                        string[] splitLine = line.Split(':');
                        if (splitLine.Length != 3) continue;
                        string name = splitLine[0];
                        string type = splitLine[1];
                        string val = splitLine[2];
                        ExposedVar evvar = null;
                        if (!EVContains(name))
                        {
                            evvar = new ExposedVar();
                        }
                        else
                        {
                            foreach (ExposedVar v in exposedVars)
                                if (v.name == name)
                                    evvar = v;
                        }
                        if (evvar == null) continue;

                        evvar.name = name;
                        evvar.varType = GetType(type);
                        evvar.value = GetValue(evvar.varType, val);

                        foreach (FieldInfo info in infos)
                            if (info.Name == evvar.name)
                                info.SetValue(null, evvar.value);
                    }
                    catch (Exception e) { Debug.LogError(e); }
                }
            }
            catch(Exception e) { Debug.LogError(e); }
        }
        catch (Exception e) { Debug.LogError(e); }
    }

    static void MakeFile()
    {
        try
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                List<string> lines = new List<string>();
                foreach (ExposedVar exposedVar in exposedVars)
                {
                    if (exposedVar.varType == VarType.NONSUPPORTED) continue;
                    lines.Add(MakeString(exposedVar));
                }
                try
                {
                    File.WriteAllLines(path, lines.ToArray());
                }
                catch { }
            }

            if (File.Exists(path))
            {
                hasFile = true;
                string[] lines = File.ReadAllLines(path);
                foreach (ExposedVar exposedVar in exposedVars)
                {
                    bool contains = false;
                    foreach (string line in lines)
                    {
                        if (line.StartsWith(exposedVar.name))
                        {
                            contains = true; break;
                        }
                    }
                    if (!contains) File.AppendAllLines(path, new string[1] { MakeString(exposedVar) });
                }
            }
        }
        catch (Exception e) { Debug.LogError(e); }
    }

    static void MakeDirectory()
    {
        try
        {
            if (!Directory.Exists(dirPath)) { Directory.CreateDirectory(dirPath); }
        }
        catch (Exception e) { Debug.LogError(e); }
    }

    static void Declare()
    {
        foreach (FieldInfo info in infos)
        {
            List<Attribute> exposed = info.GetAttributes(typeof(Expose)).ToList();
            if (exposed.Count() == 0) continue;
            if (EVContains(info.Name)) continue;
            ExposedVar exposedVar = new ExposedVar();
            exposedVar.name = info.Name;
            exposedVar.varType = VarType.NONSUPPORTED;
            if (info.FieldType == typeof(string)) exposedVar.varType = VarType.STRING;
            if (info.FieldType == typeof(int)) exposedVar.varType = VarType.INT;
            if (info.FieldType == typeof(bool)) exposedVar.varType = VarType.BOOL;
            if (info.FieldType == typeof(float)) exposedVar.varType = VarType.FLOAT;
            if (exposedVar.varType != VarType.NONSUPPORTED)
                exposedVar.value = info.GetValue(null);
            exposedVars.Add(exposedVar);
        }
    }

    static string MakeString(ExposedVar exposedVar)
    {
        return exposedVar.name + ":" + exposedVar.varType.ToString() + ":" + exposedVar.value.ToString();
    }

    static bool EVContains(string name, bool strict = false, string t = "")
    {
        foreach(ExposedVar exposedVar in exposedVars)
            if(EVVarEquals(exposedVar, name, strict, t)) 
                return true;
        
        return false;
    }

    static bool EVVarEquals(ExposedVar exposedVar, string name, bool strict = false, string t = "")
    {
        if (!strict)
        {
            if (exposedVar.name == name) return true;
        }
        else { if (exposedVar.name == name && exposedVar.varType.ToString() == t) return true; }
        return false;
    }

    static VarType GetType(string type)
    {
        if(type == VarType.FLOAT.ToString()) return VarType.FLOAT;
        else if(type == VarType.BOOL.ToString()) return VarType.BOOL;
        else if (type == VarType.INT.ToString()) return VarType.INT;
        else if (type == VarType.STRING.ToString()) return VarType.STRING;
        return VarType.NONSUPPORTED;
    }

    static object GetValue(VarType type, string val)
    {
        if (type == VarType.FLOAT) return float.Parse(val);
        if (type == VarType.BOOL) return bool.Parse(val);
        if (type == VarType.INT) return int.Parse(val);
        if (type == VarType.STRING) return val;
        return null;
    }

    #endregion
}

public class ExposedVar
{
    public string name;
    public VarType varType;
    public object value;
}

public enum VarType
{
    NONSUPPORTED,
    BOOL,
    STRING,
    INT,
    FLOAT
}

public class Expose : Attribute { }
