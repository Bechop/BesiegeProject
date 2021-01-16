// If you have FMOD in your project, uncomment the line beneath, otherwise don't
//#define FMOD
// If you have Json.NET in your project, uncomment the line beneath, otherwise don't
//#define JSONDOTNET

using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if JSONDOTNET
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

public static class Utility
{
    // -------------------- CONSTANTS --------------------

    #region Characters

    public static readonly char[] whiteSpaces = new char[] {
        (char)9,    (char)10,   (char)11,   (char)12,   (char)13,
        (char)32,   (char)133,  (char)160,  (char)5760, (char)8192,
        (char)8193, (char)8194, (char)8195, (char)8196, (char)8197,
        (char)8198, (char)8199, (char)8200, (char)8201, (char)8202,
        (char)8232, (char)8233, (char)8239, (char)8237, (char)12288 };

    #endregion

    #region Speed

    public const float MsToKmh = 3.6f;
    public const float KmhToMs = 0.277778f;

    #endregion

    // -------------------- ENUMS --------------------

    #region Direction

    public enum Side
    {
        Left, Right, Up, Down
    }

    #endregion

    #region GUI

    public enum ElementListResultType
    {
        ElementAdded = 1,
        ElementRemoved = -1
    }

    #endregion

    // -------------------- METHODS --------------------
    #region Instances

    public static T Instantiate<T>(string name = null, Transform parent = null) where T : Component
    {
        GameObject go = new GameObject(name != null ? name : typeof(T).Name);
        if (parent != null) go.transform.SetParent(parent);

        return go.AddComponent<T>();
    }

    #endregion

    #region Containers

    public static void Resize<T>(this List<T> list, int sz, T c)
    {
        int cur = list.Count;
        if (sz < cur)
            list.RemoveRange(sz, cur - sz);
        else if (sz > cur)
        {
            if (sz > list.Capacity)
                list.Capacity = sz;
            list.AddRange(Enumerable.Repeat(c, sz - cur));
        }
    }

    public static void Resize<T>(this List<T> list, int sz) where T : new()
    {
        Resize(list, sz, new T());
    }

    public static T[] Subset<T>(this T[] self, int startIndex)
    {
        return self.Subset(startIndex, self.Length - 1);
    }

    public static T[] Subset<T>(this T[] self, int startIndex, int endIndex)
    {
        UnityEngine.Debug.Assert(self != null, "Array must not be null");

        int start = Mathf.Clamp(startIndex, 0, self.Length - 1);
        int count = Mathf.Clamp(endIndex - start + 1, 1, self.Length - start);
        if (count == 0)
        {
            return System.Array.Empty<T>();
        }
        T[] array = (T[])System.Array.CreateInstance(self.GetType().GetElementType(), count);
        System.Array.Copy(self, start, array, 0, count);
        return array;
    }

    public static void SortToggle<T>(this List<T> list, System.Comparison<T> compareMethod)
    {
        bool sorted = true;

        for (int i = 0; i < list.Count - 1; i++)
        {
            if (compareMethod(list[i], list[i + 1]) > 0)
            {
                sorted = false;
                break;
            }
        }

        if (sorted)
            list.Sort((x, y) => -compareMethod(x, y));
        else
            list.Sort(compareMethod);
    }

    public static T GetRandom<T>(this List<T> list)
    {
        int c = list.Count;

        if (c == 0)
            throw new System.IndexOutOfRangeException("Can't get random element of empty list");

        return list[Random.Range(0, c)];
    }

    #endregion

    #region Lerp

    public static float LerpFactor(float lambda, float? dt = null)
    {
        return 1 - Mathf.Exp(-lambda * (dt.HasValue ? dt.Value : Time.deltaTime));
    }

    public static void Lerp(Transform dst, Transform a, Transform b, float t, bool local = false, bool localDst = false)
    {
        if (localDst) dst.localPosition = local ? Lerp(a.localPosition, b.localPosition, t) : Lerp(a.position, b.position, t);
        else dst.position = local ? Lerp(a.localPosition, b.localPosition, t) : Lerp(a.position, b.position, t);

        if (localDst) dst.localRotation = local ? Quaternion.Slerp(a.localRotation, b.localRotation, t) : Quaternion.Slerp(a.rotation, b.rotation, t);
        else dst.rotation = local ? Quaternion.Slerp(a.localRotation, b.localRotation, t) : Quaternion.Slerp(a.rotation, b.rotation, t);

        dst.localScale = Lerp(a.localScale, b.localScale, t);
    }

    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return a + (b - a) * t;
    }

    public static Vector3 Quadratic(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        return Lerp(Lerp(a, b, t), Lerp(b, c, t), t);
    }

    public static float QuadraticLength(Vector3 a, Vector3 b, Vector3 c)
    {
        float ax = a.x - 2 * b.x + c.x;
        float ay = a.y - 2 * b.y + c.y;
        float bx = 2 * b.x - 2 * a.x;
        float by = 2 * b.y - 2 * a.y;
        float A = 4 * (ax * ax + ay * ay);
        float B = 4 * (ax * bx + ay * by);
        float C = bx * bx + by * by;

        float Sabc = 2 * Mathf.Sqrt(A + B + C);
        float A_2 = Mathf.Sqrt(A);
        float A_32 = 2 * A * A_2;
        float C_2 = 2 * Mathf.Sqrt(C);
        float BA = B / A_2;

        return (A_32 * Sabc + A_2 * B * (Sabc - C_2) + (4 * C * A - B * B) * Mathf.Log((2 * A_2 + BA + Sabc) / (BA + C_2))) / (4 * A_32);
    }

    public static Vector3 Cubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        return Lerp(Quadratic(a, b, c, t), Quadratic(b, c, d, t), t);
    }

    #endregion

    #region Gizmos

    public static void DrawWireCube(Vector3 center, Vector3 size, Quaternion rotation)
    {
        Vector3 topFrontRight = (size * .5f);
        Vector3 topFrontLeft = Vector3.Scale(topFrontRight, new Vector3(-1, 1, 1));
        Vector3 topBackRight = Vector3.Scale(topFrontRight, new Vector3(1, 1, -1));
        Vector3 topBackLeft = Vector3.Scale(topFrontRight, new Vector3(-1, 1, -1));

        topFrontRight = rotation * topFrontRight;
        topFrontLeft = rotation * topFrontLeft;
        topBackRight = rotation * topBackRight;
        topBackLeft = rotation * topBackLeft;

        Gizmos.DrawLine(center + topFrontLeft, center + topFrontRight);
        Gizmos.DrawLine(center + topFrontRight, center + topBackRight);
        Gizmos.DrawLine(center + topBackRight, center + topBackLeft);
        Gizmos.DrawLine(center + topBackLeft, center + topFrontLeft);

        Gizmos.DrawLine(center - topBackRight, center - topBackLeft);
        Gizmos.DrawLine(center - topBackLeft, center - topFrontLeft);
        Gizmos.DrawLine(center - topFrontLeft, center - topFrontRight);
        Gizmos.DrawLine(center - topFrontRight, center - topBackRight);

        Gizmos.DrawLine(center + topFrontLeft, center - topBackRight);
        Gizmos.DrawLine(center + topFrontRight, center - topBackLeft);
        Gizmos.DrawLine(center + topBackRight, center - topFrontLeft);
        Gizmos.DrawLine(center + topBackLeft, center - topFrontRight);
    }

    public static void DrawCross(Vector3 center, float size, Quaternion rotation, bool direction = false)
    {
        DrawCross(center, Vector3.one * size, rotation, Color.red, Color.green, Color.blue, direction);
    }

    public static void DrawCross(Vector3 center, float size, Quaternion rotation, Color xAxisCol, Color yAxisCol, Color zAxisCol, bool direction = false)
    {
        DrawCross(center, Vector3.one * size, rotation, xAxisCol, yAxisCol, zAxisCol, direction);
    }

    public static void DrawCross(Vector3 center, Vector3 size, Quaternion rotation, bool direction = false)
    {
        DrawCross(center, size, rotation, Color.red, Color.green, Color.blue, direction);
    }

    public static void DrawCross(Vector3 center, Vector3 size, Quaternion rotation, Color xAxisCol, Color yAxisCol, Color zAxisCol, bool direction = false)
    {
        Vector3 x = rotation * Vector3.right * size.x * .5f;
        Vector3 y = rotation * Vector3.up * size.y * .5f;
        Vector3 z = rotation * Vector3.forward * size.z * .5f;

        Color prev = Gizmos.color;

        Gizmos.color = xAxisCol;
        Gizmos.DrawLine(center - x, center + x);
        if (direction) Gizmos.DrawSphere(center + x, size.x * .08f);
        Gizmos.color = yAxisCol;
        Gizmos.DrawLine(center - y, center + y);
        if (direction) Gizmos.DrawSphere(center + y, size.y * .08f);
        Gizmos.color = zAxisCol;
        Gizmos.DrawLine(center - z, center + z);
        if (direction) Gizmos.DrawSphere(center + z, size.z * .08f);

        Gizmos.color = prev;
    }

    #endregion

    #region String

    public static int IndexOfNot(this string str, char[] notOf)
    {
        return IndexOfNot(str, notOf, 0, str.Length);
    }

    public static int IndexOfNot(this string str, char[] notOf, int startIndex)
    {
        return IndexOfNot(str, notOf, startIndex, str.Length - startIndex);
    }

    public static int IndexOfNot(this string str, char[] notOf, int startIndex, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (notOf.Contains(str[startIndex + i])) continue;

            return startIndex + i;
        }

        return -1;
    }

    public static int LastIndexOfNot(this string str, char[] notOf)
    {
        return LastIndexOfNot(str, notOf, str.Length - 1, str.Length);
    }

    public static int LastIndexOfNot(this string str, char[] notOf, int startIndex)
    {
        return LastIndexOfNot(str, notOf, startIndex, startIndex + 1);
    }

    public static int LastIndexOfNot(this string str, char[] notOf, int startIndex, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (notOf.Contains(str[startIndex - i])) continue;

            return startIndex - i;
        }

        return -1;
    }

    #endregion

    #region Number

    public static string ToBinaryString(this int n)
    {
        return ToBinaryString(n, sizeof(int));
    }

    public static string ToBinaryString(this ushort n)
    {
        return ToBinaryString(n, sizeof(ushort));
    }

    static string ToBinaryString(int n, int size)
    {
        string s = "";

        for (int i = size * 8 - 1; i >= 0; i--)
        {
            s += ((n & (1 << i)) == 0) ? '0' : '1';

            if (i % 8 == 0 && i > 0) s += ' ';
        }

        return s;
    }

    public static float Map(float n, float inFrom, float inTo, float outFrom, float outTo)
    {
        return (n - inFrom) / (inTo - inFrom) * (outTo - outFrom) + outFrom;
    }

    #endregion

    #region Vector

    public static Vector3Int FloorToInt(this Vector3 vec3)
    {
        return new Vector3Int(Mathf.FloorToInt(vec3.x), Mathf.FloorToInt(vec3.y), Mathf.FloorToInt(vec3.z));
    }

    public static Vector3 Clamp(this Vector3 self, Vector3 min, Vector3 max)
    {
        self.x = Mathf.Clamp(self.x, min.x, max.x);
        self.y = Mathf.Clamp(self.y, min.y, max.y);
        self.z = Mathf.Clamp(self.z, min.z, max.z);

        return self;
    }

    public static Vector2 Clamp(this Vector2 self, Vector2 min, Vector2 max)
    {
        self.x = Mathf.Clamp(self.x, min.x, max.x);
        self.y = Mathf.Clamp(self.y, min.y, max.y);

        return self;
    }

    public static Vector2 xz(this Vector3 vec3)
    {
        return new Vector2(vec3.x, vec3.z);
    }

    public static Vector3 Flat(this Vector3 vec3, float y = 0f)
    {
        return new Vector3(vec3.x, y, vec3.z);
    }

    public static Vector3 Flat(this Vector2 vec2)
    {
        return new Vector3(vec2.x, 0f, vec2.y);
    }

    public static Vector3 RandomVector(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
    {
        return new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            Random.Range(minZ, maxZ)
        );
    }

    public static Vector3 RandomVector(float rangeX, float rangeY, float rangeZ)
    {
        return RandomVector(-rangeX, rangeX, -rangeY, rangeY, -rangeZ, rangeZ);
    }

    public static Vector3 RandomVector(float range)
    {
        return RandomVector(-range, range, -range, range, -range, range);
    }

    public static Vector3 RandomVector(Vector3 range)
    {
        return RandomVector(-range.x, range.x, -range.y, range.y, -range.z, range.z);
    }

    public static Vector3 RandomVector(Bounds bounds)
    {
        return bounds.center + RandomVector(bounds.extents);
    }

    public static float Project1D(Vector3 vec, Vector3 axis)
    {
        Vector3 proj = Vector3.Project(vec, axis);
        return proj.magnitude * Mathf.Sign(Vector3.Dot(proj, axis));
    }

    #endregion

    #region Colors

    public static Color ParseHexColor(string colorCode)
    {
        long hex = long.Parse(colorCode, System.Globalization.NumberStyles.HexNumber);

        long r = 0, g = 0, b = 0, a = 255;

        switch (colorCode.Length)
        {
            case 3:
                r = (hex & 0xF00) >> 8;
                g = (hex & 0x0F0) >> 4;
                b = (hex & 0x00F) >> 0;
                break;

            case 4:
                r = (hex & 0xF000) >> 12;
                g = (hex & 0x0F00) >> 8;
                b = (hex & 0x00F0) >> 4;
                a = (hex & 0x000F) >> 0;
                break;

            case 6:
                r = (hex & 0xFF0000) >> 16;
                g = (hex & 0x00FF00) >> 8;
                b = (hex & 0x0000FF) >> 0;
                break;

            case 8:
                r = (hex & 0xFF000000) >> 24;
                g = (hex & 0x00FF0000) >> 16;
                b = (hex & 0x0000FF00) >> 8;
                a = (hex & 0x000000FF) >> 0;
                break;
        }

        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    #endregion

    #region Quaternion

    public static Quaternion FromToQuaternion(Quaternion a, Quaternion b)
    {
        return b * Quaternion.Inverse(a);
    }

    public static Vector3 SignedEuler(this Quaternion q)
    {
        Vector3 rot = q.eulerAngles;
        rot.x = (rot.x + 180f) % 360f - 180f;
        rot.y = (rot.y + 180f) % 360f - 180f;
        rot.z = (rot.z + 180f) % 360f - 180f;

        return rot;
    }

    #endregion

    #region Rect

    public static Rect[] AreaGridLayout(Rect area, Vector2Int cellCount, Vector2 spacing)
    {
        List<Rect> rects = new List<Rect>();

        Vector2 cellSize = (area.size - (cellCount - Vector2Int.one) * spacing) / cellCount;

        for (int y = 0; y < cellCount.y; y++)
        {
            for (int x = 0; x < cellCount.x; x++)
            {
                rects.Add(new Rect(
                    area.position + new Vector2(x, y) * (spacing + cellSize),
                    cellSize
                ));
            }
        }

        return rects.ToArray();
    }

    public static Rect[] AreaCutoff(Rect area, Side side, float size, float spacing)
    {
        Rect cutoff;
        Rect rest;

        float cutoffSize, restSize;

        switch (side)
        {
            case Side.Left:
            case Side.Right:
                cutoffSize = Mathf.Min(area.width, size);
                restSize = Mathf.Max(0f, area.width - cutoffSize - spacing);
                break;

            case Side.Up:
            case Side.Down:
                cutoffSize = Mathf.Min(area.height, size);
                restSize = Mathf.Max(0f, area.height - cutoffSize - spacing);
                break;

            default:
                cutoffSize = 0f;
                restSize = 0f;
                break;
        }

        switch (side)
        {
            case Side.Left:
                cutoff = new Rect(area.x, area.y, cutoffSize, area.height);
                rest = new Rect(area.xMax - restSize, area.y, restSize, area.height);
                break;
            case Side.Right:
                cutoff = new Rect(area.xMax - cutoffSize, area.y, cutoffSize, area.height);
                rest = new Rect(area.x, area.y, restSize, area.height);
                break;
            case Side.Up:
                cutoff = new Rect(area.x, area.y, area.width, cutoffSize);
                rest = new Rect(area.x, area.yMax - restSize, area.width, restSize);
                break;
            case Side.Down:
                cutoff = new Rect(area.x, area.yMax - cutoffSize, area.width, cutoffSize);
                rest = new Rect(area.x, area.y, area.width, restSize);
                break;

            default:
                cutoff = new Rect();
                rest = new Rect();
                break;
        }

        if (side == Side.Left || side == Side.Up)
            return new Rect[] { cutoff, rest };
        else
            return new Rect[] { rest, cutoff };
    }

    public static Rect AreaMargin(Rect area, float margin)
    {
        return AreaMargin(area, new Vector2(margin, margin));
    }

    public static Rect AreaMargin(Rect area, Vector2 margin)
    {
        float x = Mathf.Min(margin.x, area.width * .5f);
        float y = Mathf.Min(margin.y, area.height * .5f);

        area.x += x;
        area.width -= 2f * x;

        area.y += y;
        area.height -= 2f * y;

        return area;
    }

    public static float AreaSize(int elemCount, float elemSize, float elemSpacing = 0f, float areaMargin = 0f)
    {
        return elemCount * elemSize + (elemCount > 0 ? elemCount - 1 : 0) * elemSpacing + 2f * areaMargin;
    }

    public static Rect Merge(this Rect self, Rect other)
    {
        float x = Mathf.Min(self.xMin, other.xMin);
        float y = Mathf.Min(self.yMin, other.yMin);

        self.Set(
            x,
            y,
            Mathf.Max(self.xMax, other.xMax) - x,
            Mathf.Max(self.yMax, other.yMax) - y
        );

        return self;
    }

    public static Rect Merge(this Rect self, Rect[] others)
    {
        foreach (var other in others)
        {
            self = self.Merge(other);
        }

        return self;
    }

    public static Rect Merge(Rect[] rects)
    {
        for (int i = 1; i < rects.Length; i++)
        {
            rects[0] = rects[0].Merge(rects[i]);
        }

        return rects[0];
    }

    public static Vector2 Clamp(this Rect rect, Vector2 point)
    {
        point.x = Mathf.Clamp(point.x, rect.x, rect.xMax);
        point.y = Mathf.Clamp(point.y, rect.y, rect.yMax);

        return point;
    }

    #endregion

    #region Coroutines

    public static Coroutine StartCoroutine(this MonoBehaviour obj, IEnumerator enumerator, System.Action<System.Exception> exceptionCatcher)
    {
        return obj.StartCoroutine(ThrowingIterator(enumerator, exceptionCatcher));
    }

    static IEnumerator ThrowingIterator(IEnumerator enumerator, System.Action<System.Exception> catcher)
    {
        while (true)
        {
            object current;

            try
            {
                if (enumerator.MoveNext() == false)
                {
                    break;
                }
                current = enumerator.Current;
            }
            catch (System.Exception e)
            {
                catcher(e);
                yield break;
            }

            yield return current;
        }
    }

    #endregion

    #region GameObject

    public static Bounds GetBounds(this GameObject go)
    {
        Bounds bounds = new Bounds(go.transform.position, Vector3.zero);

        Renderer[] renderers = go.GetComponents<Renderer>();

        foreach (var r in renderers) bounds.Encapsulate(r.bounds);

        Collider[] colliders = go.GetComponents<Collider>();

        foreach (var c in colliders) bounds.Encapsulate(c.bounds);

        foreach (Transform child in go.transform)
        {
            bounds.Encapsulate(child.gameObject.GetBounds());
        }

        return bounds;
    }

#if UNITY_EDITOR

    [MenuItem("GameObject/Create Empty Custom", priority = 0)]
    public static void NewCustomEmptyGameObject(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("Custom Game Object");
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;

        GameObject subGo = new GameObject("Visual");
        GameObjectUtility.SetParentAndAlign(subGo, go);

        subGo = new GameObject("Collision");
        GameObjectUtility.SetParentAndAlign(subGo, go);
    }

#endif

    #endregion

    #region Speed & Direction

    public static float LinearToAngularVelocity(float distToPivot, float speed)
    {
        return speed * Mathf.Rad2Deg / distToPivot;
    }

    public static float AngularToLinearVelocity(float distToPivot, float speed)
    {
        return speed * Mathf.Deg2Rad * distToPivot;
    }

    public static float GetStep(float from, float to, float step)
    {
        float dif = to - from;

        return Mathf.Sign(dif) * Mathf.Min(Mathf.Abs(dif), step);
    }

    public static int GetStep(int from, int to, int step)
    {
        int dif = to - from;

        return (int)Mathf.Sign(dif) * Mathf.Min(Mathf.Abs(dif), step);
    }

    public static float GetDir(float from, float to, float? step = null)
    {
        float dif = to - from;
        float len = 1f;

        if (step.HasValue && step.Value != 0f)
        {
            len = Mathf.Min(1f, Mathf.Abs(dif / step.Value));
        }

        return Mathf.Sign(dif) * len;
    }

    #endregion

    #region Flags

    public static bool HasFlag(this int flagList, int flag)
    {
        return (flagList & flag) == flag;
    }

    public static void SetFlag(this ref int flagList, int flag, bool active)
    {
        flagList = active ? flagList | flag : flagList & (~flag);
    }

    #endregion

    #region Documentation

    public static TooltipAttribute GetTooltip(FieldInfo field, bool inherit)
    {
        TooltipAttribute[] attributes = field.GetCustomAttributes(typeof(TooltipAttribute), inherit) as TooltipAttribute[];

        return attributes.Length > 0 ? attributes[0] : null;
    }

    #endregion

    #region Json

    #if JSONDOTNET

    //public static void Rename(this JProperty prop, string name)
    //{
    //    JToken value = prop.Value;
    //    prop.Value = null;
    //    prop.Replace(new JProperty(name, value));
    //}

    #endif

    #endregion

    #region Asset

#if UNITY_EDITOR

    public static Sprite SaveSpriteToEditorPath(Sprite sp, string path)
    {
        string dir = Path.GetDirectoryName(path);

        Directory.CreateDirectory(dir);

        File.WriteAllBytes(path, sp.texture.EncodeToPNG());
        AssetDatabase.Refresh();
        //AssetDatabase.AddObjectToAsset(sp, path);
        //AssetDatabase.SaveAssets();

        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

        ti.textureType = TextureImporterType.Sprite;
        ti.spritePixelsPerUnit = sp.pixelsPerUnit;
        ti.mipmapEnabled = false;
        EditorUtility.SetDirty(ti);
        ti.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
    }

    public static void LoadEditorTexture(ref Texture dst, string name)
    {
        if (dst == null)
        {
            dst = EditorGUIUtility.FindTexture(name);
        }
    }
#endif

    public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
    {
        string[] searchPatterns = searchPattern.Split('|');
        List<string> files = new List<string>();
        foreach (var pattern in searchPatterns)
        {
            files.AddRange(Directory.GetFiles(path, pattern, searchOption));
        }
        files.Sort();
        return files.ToArray();
    }

    public static string GetValidIndexedName(string name, string format, string formatWithIdx, System.Func<string, bool> checkNameValidity)
    {
        if (int.TryParse(name.Substring(name.LastIndexOf(' ') + 1), out int idx))
            name = name.Substring(0, name.LastIndexOf(' '));
        else
            idx = -1;

        if (!checkNameValidity(idx == -1 ? string.Format(format, name) : string.Format(formatWithIdx, name, idx)))
            do
                idx++;
            while (!checkNameValidity(string.Format(formatWithIdx, name, idx)));

        if (idx == -1)
            name = string.Format(format, name);
        else
            name = string.Format(formatWithIdx, name, idx);

        return name;
    }

    #endregion

    #region GUI

#if UNITY_EDITOR

    public static bool IconButton(Texture icon, float iconMargin = 1f, bool disabled = false)
    {
        EditorGUI.BeginDisabledGroup(disabled);

        bool pressed = GUILayout.Button(GUIContent.none, GUILayout.Width(EditorGUIUtility.singleLineHeight));

        Rect rect = GUILayoutUtility.GetLastRect();

        GUI.DrawTexture(AreaMargin(rect, iconMargin), icon, ScaleMode.ScaleToFit);

        EditorGUI.EndDisabledGroup();

        return pressed;
    }
    
    public static bool IconButton(Rect rect, Texture icon, float iconMargin = 1f, bool disabled = false)
    {
        EditorGUI.BeginDisabledGroup(disabled);

        bool pressed = GUI.Button(rect, GUIContent.none);

        GUI.DrawTexture(AreaMargin(rect, iconMargin), icon, ScaleMode.ScaleToFit);

        EditorGUI.EndDisabledGroup();

        return pressed;
    }

    public static Rect GetCurrentRect(float? height = null)
    {
        Rect rect = GUILayoutUtility.GetLastRect();

        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
        rect.height = height ?? EditorGUIUtility.singleLineHeight;

        return rect;
    }

    public static void DrawBorderRect(Rect rect, Color color, float size)
    {
        EditorGUI.DrawRect(new Rect(rect.x - size, rect.y - size, size, rect.height + 2f * size), color); // Left
        EditorGUI.DrawRect(new Rect(rect.x + rect.width, rect.y - size, size, rect.height + 2f * size), color); // Right
        EditorGUI.DrawRect(new Rect(rect.x, rect.y - size, rect.width, size), color); // Top
        EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height, rect.width, size), color); // Bot
    }

    public static Vector2 ScrollableElementList(
        int elemCount, int maxElemCountShown, float elemHeight, Vector2 scrollPos, System.Func<int, ElementListResult> showElemCallback,
        string title, Texture[] topBarIcons, out int topButtonPressed, float topBarIconMargin,
        Color backgroundColor, Color borderColor, float borderSize)
    {
        // Top bar
        {
            Rect rect = EditorGUILayout.GetControlRect();

            rect = EditorGUI.PrefixLabel(rect, new GUIContent(title));

            // Buttons
            topButtonPressed = -1;

            if (topBarIcons != null)
            {
                Rect button = rect;
                button.width = button.height;

                for (int i = 0; i < topBarIcons.Length; i++)
                {
                    if (IconButton(button, topBarIcons[i], topBarIconMargin))
                    {
                        topButtonPressed = i;
                        break;
                    }

                    button.x += button.width + EditorGUIUtility.standardVerticalSpacing;
                }
            }
        }

        float areaHeight = AreaSize(Mathf.Min(elemCount, maxElemCountShown), elemHeight, EditorGUIUtility.standardVerticalSpacing);

        Rect view = GetCurrentRect(areaHeight);

        // Background
        EditorGUI.DrawRect(view, backgroundColor);

        // Scroll
        bool isScrollable = elemCount > maxElemCountShown;

        if (isScrollable)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(areaHeight));
        }
        else
        {
            scrollPos = Vector2.zero;
        }

        // Elements
        for (int i = 0; i < elemCount; i++)
        {
            elemCount += showElemCallback(i).elementCountDelta;
        }

        // Scroll end
        if (isScrollable)
        {
            EditorGUILayout.EndScrollView();
        }

        // Borders
        DrawBorderRect(view, borderColor, borderSize);

        return scrollPos;
    }

    public static int LayoutLayerMaskField(string label, int mask)
    {
        List<(string, int)> layers = new List<(string name, int index)>();

        int nameMask = 0;

        for (int i = 0; i < 32; i++)
        {
            string cur = LayerMask.LayerToName(i);

            if (cur.Length == 0) continue;

            nameMask.SetFlag(1 << layers.Count, mask.HasFlag(1 << i));

            layers.Add((cur, i));
        }

        nameMask = EditorGUILayout.MaskField(label, nameMask, layers.Select(l => l.Item1).ToArray());

        mask = 0;

        for (int i = 0; i < layers.Count; i++)
        {
            int nameFlag = 1 << i;
            int layerFlag = 1 << layers[i].Item2;

            mask.SetFlag(layerFlag, nameMask.HasFlag(nameFlag));
        }

        return mask;
    }

    public static void Separator(float thickness = 1f, bool vertical = false)
    {
        Rect rect;

        if (vertical)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(thickness), GUILayout.MaxHeight(9999f));

            rect = GUILayoutUtility.GetRect(thickness, 9999f);

            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(9999f), GUILayout.Height(thickness));

            rect = GUILayoutUtility.GetRect(1f, thickness);

            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.DrawRect(rect, Color.black * .8f);
    }

#endif

    public static Rect GetScreenRectFromBounds(Camera cam, Bounds b)
    {
        Vector3[] pts = new Vector3[8];

        pts[0] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
        pts[1] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
        pts[2] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
        pts[3] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
        pts[4] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
        pts[5] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
        pts[6] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
        pts[7] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));

        return GetScreenRectFromPoints(cam, pts);
    }

    public static Rect GetScreenRectFromCube(Camera cam, Vector3 center, Vector3 size, Quaternion rotation)
    {
        Vector3 topFrontRight = (size * .5f);
        Vector3 topFrontLeft = Vector3.Scale(topFrontRight, new Vector3(-1, 1, 1));
        Vector3 topBackRight = Vector3.Scale(topFrontRight, new Vector3(1, 1, -1));
        Vector3 topBackLeft = Vector3.Scale(topFrontRight, new Vector3(-1, 1, -1));

        topFrontRight = rotation * topFrontRight;
        topFrontLeft = rotation * topFrontLeft;
        topBackRight = rotation * topBackRight;
        topBackLeft = rotation * topBackLeft;

        Vector3[] pts = new Vector3[8];

        pts[0] = cam.WorldToScreenPoint(center + topFrontLeft);
        pts[1] = cam.WorldToScreenPoint(center + topFrontRight);
        pts[2] = cam.WorldToScreenPoint(center + topBackLeft);
        pts[3] = cam.WorldToScreenPoint(center + topBackRight);
        pts[4] = cam.WorldToScreenPoint(center - topFrontLeft);
        pts[5] = cam.WorldToScreenPoint(center - topFrontRight);
        pts[6] = cam.WorldToScreenPoint(center - topBackLeft);
        pts[7] = cam.WorldToScreenPoint(center - topBackRight);

        return GetScreenRectFromPoints(cam, pts);
    }

    public static Rect GetScreenRectFromPoints(Camera cam, Vector3[] pts)
    {
        Vector3 min = pts[0];
        Vector3 max = pts[0];
        for (int i = 1; i < pts.Length; i++)
        {
            min = Vector3.Min(min, pts[i]);
            max = Vector3.Max(max, pts[i]);
        }

        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }

    #endregion

    #region Type

    public static bool IsSubclassOfRawGeneric(this System.Type toCheck, System.Type generic)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return true;
            }
            toCheck = toCheck.BaseType;
        }
        return false;
    }

    public static System.Type[] GetSubtypes(this System.Type of, bool allowAbstract = false)
    {
        return of.Assembly.GetTypes().Where(type => (!type.IsAbstract || allowAbstract) && type.IsSubclassOf(of)).ToArray();
    }

    #endregion

    // -------------------- CLASSES & STRUCTS --------------------

    #region Singleton Pattern

    public class UniqueInstance<T> : MonoBehaviour where T : UniqueInstance<T>
    {
        static T instance = null;

        [SerializeField]
        public bool useAsUniqueInstance = true;

        public static void Init()
        {
            if (instance == null)
            {
                T[] instances = FindObjectsOfType<T>();

                foreach (var i in instances)
                {
                    if ((i as UniqueInstance<T>).useAsUniqueInstance)
                    {
                        instance = i;
                        break;
                    }
                }
            }
        }

        public static void Refresh(T target = null)
        {
            instance = target;

            Init();
        }

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    Init();
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (useAsUniqueInstance) Refresh(this as T);
        }
    }

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T instance = null;

        public static void Init()
        {
            if (instance == null)
            {
                instance = Utility.Instantiate<T>();
                DontDestroyOnLoad(instance.gameObject);
            }
        }

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    Init();
                }

                return instance;
            }
        }
    }

    public class SingletonData<InstType, DataType> : ScriptableObject
        where InstType : LoadedSingleton<InstType, DataType>
        where DataType : SingletonData<InstType, DataType>
    {
        public virtual void OnLoaded()
        {
        }
    }

    public class LoadedSingleton<InstType, DataType> : MonoBehaviour
        where InstType : LoadedSingleton<InstType, DataType>
        where DataType : SingletonData<InstType, DataType>
    {
        static InstType instance = null;

        public static bool Initiated => instance != null;

        protected DataType data;
        public DataType Data { get => data; }

        public static void Init()
        {
            if (!Initiated)
            {
                instance = Utility.Instantiate<InstType>();
                DontDestroyOnLoad(instance.gameObject);
                
                instance.LoadData(typeof(InstType).Name + "Data");

                instance.LateAwake();
            }
        }
        public static InstType Instance
        {
            get
            {
                if (!Initiated)
                {
                    Init();
                }

                return instance;
            }
        }

        public virtual void LoadData(string name)
        {
            data = Resources.Load<DataType>(name);

            if (data == null)
                throw new System.Exception("Couldn't load singleton data : " + typeof(InstType).Name + "Data\nAdd a singleton data inside the resource folder.\n");

            data.OnLoaded();
        }

        protected virtual void LateAwake()
        {
        }
    }

    #endregion

    #region Number

    public interface IRange<T>
    {
        bool Contains(T _n, bool _strict = false);

        T Clamp(T _n);
    }

    public class Range<T> : IRange<T> where T : System.IComparable<T>
    {
        [SerializeField, Delayed]
        protected T min;
        [SerializeField, Delayed]
        protected T max;

        public T Min { get => min; set => min = value; }
        public T Max { get => max; set => max = value; }

        public Range(T _min, T _max)
        {
            min = _min;
            max = _max;
        }

        public bool Contains(T _n, bool _strict = false)
        {
            return _strict ? min.CompareTo(_n) < 0 && _n.CompareTo(max) < 0 : min.CompareTo(_n) <= 0 && _n.CompareTo(max) <= 0;
        }

        public T Clamp(T _n)
        {
            return _n.CompareTo(min) < 0
                ? min
                : _n.CompareTo(max) > 0
                ? max
                : _n;
        }
    }

    [System.Serializable]
    public class RangeFloat : Range<float>
    {
        public RangeFloat(float _min, float _max) :
            base(_min, _max)
        {
        }

        public float Extent { get => max - min; }

        /// <summary>
        /// input mapped to the range (0 = min, 1 = max)
        /// </summary>
        /// <param name="t">progress, between 0 (min) and 1 (max)</param>
        /// <returns>mapped input</returns>
        public float Evaluate(float t)
        {
            return min + Extent * Mathf.Clamp01(t);
        }

        // input between min and max, mapped to [0, 1] range (min = 0, max = 1)
        public float EvaluateProgress(float t)
        {
            float e = Extent;

            if (e == 0f) return 0f;

            return Mathf.Clamp01((t - min) / e);
        }

        public float GetRandom()
        {
            return Evaluate(Random.value);
        }
    }

    [System.Serializable]
    public class RangeInt : Range<int>
    {
        public RangeInt(int _min, int _max) :
            base(_min, _max)
        {
        }

        public int Extent { get => max - min; }

        public int Evaluate(float t)
        {
            return min + (int)(Extent * Mathf.Clamp01(t));
        }

        public int GetRandom(bool minIncluded = true, bool maxIncluded = true)
        {
            return Random.Range(min + (minIncluded ? 0 : 1), max + (maxIncluded ? 1 : 0));
        }
    }

    public class Clamped<T> : Range<T> where T : System.IComparable<T>
    {
        [SerializeField, Delayed]
        protected T val;

        public T value
        {
            get => val;
            set
            {
                val = Clamp(value);
            }
        }

        public new T Min
        {
            get => min;

            set
            {
                min = value;
                if (min.CompareTo(max) > 0) max = min;
                if (min.CompareTo(val) > 0) val = min;
            }
        }

        public new T Max
        {
            get => max;

            set
            {
                max = value;
                if (max.CompareTo(min) < 0) min = max;
                if (max.CompareTo(val) < 0) val = max;
            }
        }

        public Clamped(T _val, T _min, T _max) :
            base(_min, _max)
        {
            val = _val;
        }
    }

    [System.Serializable]
    public class ClampedFloat : Clamped<float>
    {
        public ClampedFloat(float _val, float _min, float _max) : base(_val, _min, _max)
        {
        }

        public float Progress
        {
            get
            {
                return value / (value < 0f ? Mathf.Abs(Min) : Max);
            }
        }

        public float Evaluate(float _progress)
        {
            _progress = Mathf.Clamp(_progress, -1f, 1f);

            return _progress * (_progress < 0f ? Mathf.Abs(Min) : Max);
        }
    }

    [System.Serializable]
    public class ClampedInt : Clamped<int>
    {
        public ClampedInt(int _val, int _min, int _max) : base(_val, _min, _max)
        {
        }
    }

    public interface IMappedRange<T>
    {
        T Map(T _n);
    }

    [System.Serializable]
    public class MappedRangeFloat : IMappedRange<float>
    {
        [SerializeField]
        public RangeFloat input = null;

        [SerializeField]
        public RangeFloat output = null;

        public float Map(float _n)
        {
            return output.Evaluate(input.EvaluateProgress(_n));
        }
    }

    [System.Serializable]
    public class MappedFloat : IMappedRange<float>
    {
        [TwoLine(2)]
        [SerializeField]
        public float inFrom = 0f;

        [HideInInspector]
        [SerializeField]
        public float inTo = 1f;

        [TwoLine(2)]
        [SerializeField]
        public float outFrom = 0f;

        [HideInInspector]
        [SerializeField]
        public float outTo = 1f;

        public float Map(float _n)
        {
            return (_n - inFrom) / (inTo - inFrom) * (outTo - outFrom) + outFrom;
        }
    }

    #endregion

    #region Position

    public class OffsetTransform
    {
        public Vector3 pos = Vector3.zero;
        public Quaternion rot = Quaternion.identity;

        public OffsetTransform()
        {
        }

        public OffsetTransform(Transform t)
        {
            pos = t.position;
            rot = t.rotation;
        }

        public OffsetTransform(Offset t)
        {
            if (t.attached)
            {
                pos = t.attached.TransformPoint(t.pos);
                rot = t.attached.rotation * Quaternion.Euler(t.rot);
            }
            else
            {
                pos = t.pos;
                rot = Quaternion.Euler(t.rot);
            }
        }

        public static void Lerp(Transform src, OffsetTransform dst, float t)
        {
            src.position = Vector3.Lerp(src.position, dst.pos, t);
            src.rotation = Quaternion.Lerp(src.rotation, dst.rot, t);
        }

        public static void Set(Transform src, OffsetTransform dst)
        {
            src.position = dst.pos;
            src.rotation = dst.rot;
        }
    }

    [System.Serializable]
    public class Offset
    {
        public Vector3 pos = Vector3.zero;
        public Vector3 rot = Vector3.zero;

        public Transform attached = null;

        public Offset(Transform t = null)
        {
            attached = t;
        }

        public OffsetTransform GetTransform()
        {
            return new OffsetTransform(this);
        }
    }

    #endregion

    #region Rotation

    [System.Serializable]
    public class RangeEuler : IRange<Quaternion>
    {
        [SerializeField]
        public RangeFloat x;
        [SerializeField]
        public RangeFloat y;
        [SerializeField]
        public RangeFloat z;

        public Quaternion Clamp(Quaternion _n)
        {
            Vector3 rot = _n.SignedEuler();

            return Quaternion.Euler(Clamp(rot));
        }

        public Vector3 Clamp(Vector3 _n)
        {
            _n.x = x.Clamp(_n.x);
            _n.y = y.Clamp(_n.y);
            _n.z = z.Clamp(_n.z);

            return _n;
        }

        public bool Contains(Quaternion _n, bool _strict = false)
        {
            Vector3 rot = _n.SignedEuler();

            return Contains(rot);
        }

        public bool Contains(Vector3 _n, bool _strict = false)
        {
            return x.Contains(_n.x, _strict) && y.Contains(_n.y, _strict) && z.Contains(_n.z, _strict);
        }
    }

    #endregion

    #region Time

    [System.Serializable]
    public class Timer
    {
        [SerializeField]
        protected float duration = 1f;

        /// <summary> The total duration in seconds </summary>
        public float Duration { get => duration; set => duration = value; }
        /// <summary> Normalized between 0 and 1, in percentage (1 mean it's over, 0 when not started) </summary>
        public float Progress { get => IsStarted ? Mathf.Clamp01(1f - Remaining / duration) : 0f; }
        /// <summary> Normalized between 0 and 1, in percentage (1 mean it's over, 1 when not started) </summary>
        public float ProgressRaw { get => Mathf.Clamp01(1f - Remaining / duration); }
        /// <summary> Normalized between 0 and 1, in percentage (0 mean it's over, 0 when not started) </summary>
        public float Cooldown { get => Mathf.Clamp01(Remaining / duration); }
        /// <summary> The time remaining in seconds </summary>
        public float Remaining { get; private set; } = 0f;
        /// <summary> The time elapsed in seconds </summary>
        public float Elapsed { get => duration - Remaining; }

        [SerializeField]
        bool isStarted = false;

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        bool repaintTrigger = false;
#endif

        public bool IsStarted
        {
            get => isStarted;

            set
            {
                if (isStarted && !value)
                {
                    Stop();
                }
                else if (!isStarted && value)
                {
                    Start();
                }
            }
        }

        public Timer()
        {
        }

        public Timer(float _duration, bool _startDirectly = false)
        {
            duration = _duration;
            if (_startDirectly) Start();
        }

        public virtual bool Timeout(bool _looping, float? dt = null)
        {
            if (!isStarted) return false;

            Remaining -= (dt != null) ? (dt.Value) : (Time.deltaTime);

            bool ended = Remaining <= 0f;

            if (ended)
            {
                if (_looping)
                {
                    Remaining += duration;
                }
                else
                {
                    Stop();
                }
            }

#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                repaintTrigger = !repaintTrigger;
            }
#endif

            return ended;
        }

        public virtual bool Timeout(float? dt = null)
        {
            return Timeout(false, dt);
        }

        public virtual void Start()
        {
            Remaining = duration;
            isStarted = true;
        }

        public virtual void Stop()
        {
            Remaining = 0f;
            isStarted = false;
        }
    }

    public class TimerBuffer<T> : Timer
    {
        public T data;

        public TimerBuffer() : base()
        {
        }

        public TimerBuffer(T _data) : base()
        {
            data = _data;
        }

        public TimerBuffer(float _duration, bool _startDirectly = false) : base(_duration, _startDirectly)
        {
        }

        public TimerBuffer(T _data, float _duration, bool _startDirectly = false) : base(_duration, _startDirectly)
        {
            data = _data;
        }

    }

    [System.Serializable]
    public class TimerBuffer : TimerBuffer<object>
    {

    }

    #endregion

    #region GameObject

    public interface ISpawnElement
    {
        void ApplySpawnParams(Vector3 pos, Quaternion rot, Transform parent);
    }

    [System.Serializable]
    public class SpawnList
    {
        public List<GameObject> objects = new List<GameObject>();



        public void Spawn(Vector3 pos, Quaternion rot, Transform parent = null)
        {
            foreach (var elem in objects)
            {
                GameObject obj = Object.Instantiate(elem);

                if (obj.TryGetComponent(out ISpawnElement spElem))
                {
                    spElem.ApplySpawnParams(pos, rot, parent);
                }
                else
                {
                    obj.transform.parent = parent;
                    obj.transform.position = pos;
                    obj.transform.rotation = rot;
                    obj.transform.localScale = Vector3.one;
                }
            }
        }
    }

    #endregion

    #region Asset

    [System.Serializable]
    public class SceneReference
    {
        public string name = "";
        public int buildIndex = -1;
    }

    #endregion

    #region Attributes

    public class TwoLineAttribute : PropertyAttribute
    {
        public uint count;

        public TwoLineAttribute(uint _count)
        {
            count = _count;
        }
    }

    public class LockedAttribute : PropertyAttribute
    {
        public LockedAttribute()
        {
        }
    }

    public class LineAttribute : PropertyAttribute
    {
        public float paddingAbove;
        public float paddingUnder;
        public float thickness;
        public Color color;

        public LineAttribute(float _thickness = 2f, float _paddingAbove = 5f, float _paddingUnder = 5f, float _r = 0.5f, float _g = 0.5f, float _b = 0.5f, float _a = 1f)
        {
            paddingAbove = _paddingAbove;
            paddingUnder = _paddingUnder;
            thickness = _thickness;
            color = new Color(_r, _g, _b, _a);
        }
    }

    public class VisibleIfAttribute : PropertyAttribute
    {
        public string fieldName = "";
        public object equalTo = null;
        public bool inverted = false;

        public VisibleIfAttribute(string _fieldName, object _equalTo, bool _inverted = false)
        {
            fieldName = _fieldName;
            equalTo = _equalTo;
            inverted = _inverted;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property |
    System.AttributeTargets.Class | System.AttributeTargets.Struct, Inherited = true)]
    public class ConditionalHideAttribute : PropertyAttribute
    {
        public string conditionalSourceField;
        public bool showIfTrue;
        public int enumIndex;

        public ConditionalHideAttribute(string boolVariableName, bool showIfTrue)
        {
            conditionalSourceField = boolVariableName;
            this.showIfTrue = showIfTrue;
        }

        public ConditionalHideAttribute(string enumVariableName, int enumIndex)
        {
            conditionalSourceField = enumVariableName;
            this.enumIndex = enumIndex;
        }

    }

    public class OnChangedCallbackAttribute : PropertyAttribute
    {
        public string callbackName = "";

        public OnChangedCallbackAttribute(string _callbackName)
        {
            callbackName = _callbackName;
        }
    }

    public class OnChangedAssignAttribute : PropertyAttribute
    {
        public string TargetName { get; private set; }
        public string CallbackName { get; private set; }

        public OnChangedAssignAttribute(string target, string callback)
        {
            TargetName = target;
            CallbackName = callback;
        }
    }
 
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class SerializePropertyAttribute : PropertyAttribute
    {
        public string PropertyName { get; private set; }

        public SerializePropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
 
    public class GroupAttribute : PropertyAttribute
    {
        public string GroupName { get; private set; }
        public int PropertyCount { get; private set; }

        public GroupAttribute(string name, int size)
        {
            GroupName = name;
            PropertyCount = size;
        }
    }

    public class EnforceTypeAttribute : PropertyAttribute
    {
        public System.Type type;

        public EnforceTypeAttribute(System.Type enforcedType)
        {
            type = enforcedType;
        }
    }

    #endregion

    #region Sound

#if FMOD
    [System.Serializable]
    public class AudioEvent
    {
        [SerializeField]
        [FMODUnity.EventRef]
        string path = "";
        [SerializeField]
        public float cooldown = 0f;

        Stopwatch stopwatch = new Stopwatch();

        FMOD.Studio.EventInstance instance;

        public FMOD.Studio.EventInstance Instance { get => instance; }

        bool isSetup = false;

        public float Duration
        {
            get
            {
                int length;
                FMOD.Studio.EventDescription desc;

                if (instance.getDescription(out desc) == FMOD.RESULT.OK
                 && desc.getLength(out length) == FMOD.RESULT.OK)
                {
                    return length / 1000f;
                }

                return -1f;
            }
        }

        public bool IsPlaying { get { FMOD.Studio.PLAYBACK_STATE state; instance.getPlaybackState(out state); return state == FMOD.Studio.PLAYBACK_STATE.PLAYING; } }

        public AudioEvent()
        {
        }

        public AudioEvent(string _path, float _cooldown = 0f)
        {
            path = _path;
            cooldown = _cooldown;
        }

        public void Init()
        {
            instance = FMODUnity.RuntimeManager.CreateInstance(path);

            isSetup = true;
        }

        public void Release()
        {
            //Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();

            isSetup = false;
        }

        public void Play(bool _ignoreCooldown = false)
        {
            if (!isSetup)
            {
                UnityEngine.Debug.LogError("Trying to play AudioEvent that is not setup !\nEvent path : " + path);
                return;
            }

            if (!stopwatch.IsRunning || stopwatch.ElapsedMilliseconds / 1000f >= cooldown || _ignoreCooldown)
            {
                instance.start();

                stopwatch.Restart();
            }
        }

        public void Play(Vector3 _pos, bool _ignoreCooldown = false)
        {
            FMOD.VECTOR pos = new FMOD.VECTOR() { x = _pos.x, y = _pos.y, z = _pos.z };
            FMOD.ATTRIBUTES_3D attrib = new FMOD.ATTRIBUTES_3D() { position = pos };

            instance.set3DAttributes(attrib);

            Play(_ignoreCooldown);
        }

        public void PlayDelayed(MonoBehaviour _on, float _seconds, bool _ignoreCooldown = false)
        {
            _on.StartCoroutine(PlayCoroutine(_seconds, _ignoreCooldown));
        }

        IEnumerator PlayCoroutine(float _seconds, bool _ignoreCooldown = false)
        {
            yield return new WaitForSeconds(_seconds);

            Play(_ignoreCooldown);
        }

        public void Stop(FMOD.Studio.STOP_MODE _mode)
        {
            instance.stop(_mode);
            stopwatch.Reset();
        }

        public void ResetCooldown()
        {
            stopwatch.Reset();
        }
    }
#endif

    #endregion

    #region GUI

    public struct ElementListResult
    {
        public static ElementListResult SameElements { get => new ElementListResult(0); }
        public static ElementListResult OneRemoved { get => new ElementListResult(-1); }
        public static ElementListResult OneAdded { get => new ElementListResult(1); }

        public int elementCountDelta;

        public ElementListResult(int _elementCountDelta)
        {
            elementCountDelta = _elementCountDelta;
        }

        public ElementListResult(ElementListResultType type, int count)
        {
            elementCountDelta = (int)type * count;
        }

        public static ElementListResult operator* (ElementListResult elem, int count)
        {
            elem.elementCountDelta *= count;

            return elem;
        }
    }

    [System.Serializable]
    public class ScrambledText
    {
        string original = "";
        string scrambled = "";

        List<int> order = new List<int>();



        public string scrambleChars = "#@|/\\!+_*\"~[}$%";

        public string Original { get => original; set { original = value; if (original == null || original.Length != order.Count) RandomizeOrder(); } }
        public string Scrambled { get => scrambled; set { scrambled = value; if (scrambled == null) RandomizeScrambled(); } }



        public void RandomizeOrder(int? seed = null)
        {
            int len = original?.Length ?? 0;

            if (seed.HasValue) Random.InitState(seed.Value);

            order.Clear();
            for (int i = 0; i < len; i++)
            {
                order.Insert(Random.Range(0, i + 1), i);
            }
        }

        public void RandomizeScrambled(int? seed = null)
        {
            int len = original?.Length ?? 0;
            int slen = scrambleChars.Length;

            if (seed.HasValue) Random.InitState(seed.Value);

            scrambled = "";
            for (int i = 0; i < len; i++)
            {
                scrambled += scrambleChars[Random.Range(0, slen)];
            }
        }

        /// <summary>
        /// 0 is full scrambled, 1 full revealed
        /// </summary>
        public string GetRevealedAt(float t)
        {
            return GetScrambledAt(1f - t);
        }

        /// <summary>
        /// 0 is full revealed, 1 full scrambled
        /// </summary>
        public string GetScrambledAt(float t)
        {
            int slen = scrambled.Length;
            int count = Mathf.RoundToInt(Mathf.Clamp01(t) * original.Length);

            char[] str = original.ToCharArray();
            for (int i = 0; i < count; i++)
            {
                int idx = order[i];
                str[idx] = scrambled[idx % slen];
            }

            return new string(str);
        }
    }

    #endregion

    #region Input

    public class AxisInput
    {
        public static float deadzone = .1f;

        public string axis { get; private set; } = "";

        bool anyPressed = false;
        bool anyHolded = false;
        bool anyReleased = false;

        bool posPressed = false;
        bool posHolded = false;
        bool posReleased = false;

        bool negPressed = false;
        bool negHolded = false;
        bool negReleased = false;

        public bool AnyPressed => anyPressed;
        public bool AnyHolded => anyHolded;
        public bool AnyReleased => anyReleased;

        public bool PosPressed => posPressed;
        public bool PosHolded => posHolded;
        public bool PosReleased => posReleased;

        public bool NegPressed => negPressed;
        public bool NegHolded => negHolded;
        public bool NegReleased => negReleased;

        public bool used { get; private set; } = false;

        public float value { get; private set; } = 0f;
        public float raw { get; private set; } = 0f;

        public float? deadzoneOverride { get; set; } = null;
        public bool useRaw { get; set; } = true;

        public AxisInput(string axis, float? deadzoneOverride = null, bool useRaw = true)
        {
            this.axis = axis;
            this.deadzoneOverride = deadzoneOverride;
            this.useRaw = useRaw;
        }

        void Update(ref bool pressed, ref bool holded, ref bool released, bool active)
        {
            if (pressed) pressed = false;
            if (released) released = false;

            if (!holded && !used)
            {
                if (active)
                {
                    pressed = true;
                    holded = true;
                }
            }
            else
            {
                if (!active)
                {
                    holded = false;
                    if (!used) released = true;
                }
            }
        }

        public void Update()
        {
            value = Input.GetAxis(axis);
            raw = Input.GetAxisRaw(axis);

            float deadzoneActual = deadzoneOverride.GetValueOrDefault(deadzone);

            float curVal = useRaw ? raw : value;
            bool active = Mathf.Abs(curVal) > deadzoneActual;

            Update(ref anyPressed, ref anyHolded, ref anyReleased, active);
            Update(ref posPressed, ref posHolded, ref posReleased, active && curVal >= 0f);
            Update(ref negPressed, ref negHolded, ref negReleased, active && curVal <= 0f);

            if (used && !active) used = false;
        }

        public bool Use(bool release = true)
        {
            if (!anyHolded || used) return false;

            used = true;
            anyPressed = false;
            anyHolded = false;
            anyReleased = release;

            if (posHolded) posReleased = release;
            posPressed = false;
            posHolded = false;

            if (negHolded) negReleased = release;
            negPressed = false;
            negHolded = false;

            return true;
        }
    }

    #endregion

    // -------------------- EDITOR ONLY --------------------

#if UNITY_EDITOR
    #region SaveMesh
    [MenuItem("CONTEXT/MeshFilter/Save Mesh...")]
    public static void SaveMeshInPlace(MenuCommand menuCommand)
    {
        MeshFilter mf = menuCommand.context as MeshFilter;
        Mesh m = mf.sharedMesh;
        SaveMesh(m, m.name, false, true);
    }

    [MenuItem("CONTEXT/MeshFilter/Save Mesh As New Instance...")]
    public static void SaveMeshNewInstanceItem(MenuCommand menuCommand)
    {
        MeshFilter mf = menuCommand.context as MeshFilter;
        Mesh m = mf.sharedMesh;
        SaveMesh(m, m.name, true, true);
    }
    #endregion

    public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

        if (optimizeMesh)
            MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }

    #region SerializedProperty

    public static object GetTargetObjectWithProperty(SerializedProperty prop)
    {
        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements.Take(elements.Length))
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }
        }
        return obj;
    }

    private static object GetValue_Imp(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();

        while (type != null)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }
        return null;
    }

    private static object GetValue_Imp(object source, string name, int index)
    {
        var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
        if (enumerable == null) return null;
        var enm = enumerable.GetEnumerator();
        //while (index-- >= 0)
        //    enm.MoveNext();
        //return enm.Current;

        for (int i = 0; i <= index; i++)
        {
            if (!enm.MoveNext()) return null;
        }
        return enm.Current;
    }

    public static object GetPropertyParentObject(SerializedProperty property)
    {
        int lastDot = property.propertyPath.LastIndexOf('.');

        if (lastDot != -1)
        {
            string path = property.propertyPath.Substring(0, lastDot);

            return GetTargetObjectWithProperty(property.serializedObject.FindProperty(path));
        }
        else
        {
            return property.serializedObject.targetObject;
        }
    }

    public static SerializedProperty FindSiblingProperty(this SerializedProperty property, string name)
    {
        int lastDot = property.propertyPath.LastIndexOf('.');

        string path = "";

        if (lastDot != -1)
            path = property.propertyPath.Substring(0, lastDot + 1);

        path += name;

        return property.serializedObject.FindProperty(path);
    }

    #endregion

    #region UI

    public static int SpriteListGUI(List<Sprite> sprites, float zoom, float regionWidth, float texSize, float border, int? selectedIndex = null)
    {
        Event e = Event.current;

        float zoomedSize = Mathf.Min(regionWidth, texSize * zoom);
        float Yoffset = EditorGUILayout.GetControlRect().y;

        int texPerLine = (int)(regionWidth / zoomedSize);

        Vector2 borderOffset = new Vector2(border, border);

        int i = 0;
        foreach (Sprite spr in sprites)
        {
            Rect sprRect = spr.textureRect;

            Texture2D tex = spr.texture;

            float max = Mathf.Max(sprRect.width, sprRect.height);

            Rect rect = new Rect(
                (i % texPerLine) * zoomedSize,
                Yoffset + zoomedSize * (1f - sprRect.height / max) * .5f + (i / texPerLine) * zoomedSize,
                zoomedSize * sprRect.width / max,
                zoomedSize * sprRect.height / max
            );

            Rect texRect = new Rect(
                rect.position + borderOffset,
                rect.size - borderOffset * 2f
            );

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                selectedIndex = i;
            }

            if (i == selectedIndex)
            {
                EditorGUI.DrawRect(rect, Color.yellow);
            }

            Rect texCoords = sprRect;

            texCoords.xMin /= tex.width;
            texCoords.xMax /= tex.width;
            texCoords.yMin /= tex.height;
            texCoords.yMax /= tex.height;

            GUI.DrawTextureWithTexCoords(texRect, tex, texCoords);

            i++;
        }

        GUILayout.Space(((i - 1) / texPerLine + 1f) * zoomedSize);

        return selectedIndex.Value;
    }

    public static object DrawProperty(Rect position, SerializedPropertyType propertyType, System.Type type, object value, GUIContent label)
    {
        switch (propertyType)
        {
            case SerializedPropertyType.Integer:
                return EditorGUI.IntField(position, label, (int)value);
            case SerializedPropertyType.Boolean:
                return EditorGUI.Toggle(position, label, (bool)value);
            case SerializedPropertyType.Float:
                return EditorGUI.FloatField(position, label, (float)value);
            case SerializedPropertyType.String:
                return EditorGUI.TextField(position, label, (string)value);
            case SerializedPropertyType.Color:
                return EditorGUI.ColorField(position, label, (Color)value);
            case SerializedPropertyType.ObjectReference:
                return EditorGUI.ObjectField(position, label, (Object)value, type, true);
            case SerializedPropertyType.ExposedReference:
                return EditorGUI.ObjectField(position, label, (Object)value, type, true);
            case SerializedPropertyType.LayerMask:
                return EditorGUI.LayerField(position, label, (int)value);
            case SerializedPropertyType.Enum:
                return EditorGUI.EnumPopup(position, label, (System.Enum)value);
            case SerializedPropertyType.Vector2:
                return EditorGUI.Vector2Field(position, label, (Vector2)value);
            case SerializedPropertyType.Vector3:
                return EditorGUI.Vector3Field(position, label, (Vector3)value);
            case SerializedPropertyType.Vector4:
                return EditorGUI.Vector4Field(position, label, (Vector4)value);
            case SerializedPropertyType.Rect:
                return EditorGUI.RectField(position, label, (Rect)value);
            case SerializedPropertyType.AnimationCurve:
                return EditorGUI.CurveField(position, label, (AnimationCurve)value);
            case SerializedPropertyType.Bounds:
                return EditorGUI.BoundsField(position, label, (Bounds)value);
            default:
                throw new System.NotImplementedException("Unimplemented propertyType " + propertyType + ".");
        }
    }

    public static void PropertyField(Object[] targets, PropertyInfo property, SerializedPropertyType propertyType, SerializedProperty serializedProperty = null)
    {
        Rect position = EditorGUILayout.GetControlRect();

        GUIContent label = new GUIContent(ObjectNames.NicifyVariableName(property.Name));

        if (serializedProperty != null)
            EditorGUI.BeginProperty(position, label, serializedProperty);

        if (property == null)
        {
            EditorGUI.LabelField(position, "Error: could not retrieve property.");
        }
        else if (targets.Length == 0)
        {
            EditorGUI.LabelField(position, "Error: no targets.");
        }
        else
        {
            object value = property.GetValue(targets[0]);

            EditorGUI.BeginChangeCheck();
            value = DrawProperty(position, propertyType, property.PropertyType, value, label);

            if (EditorGUI.EndChangeCheck() && property != null)
            {
                foreach (var t in targets)
                {
                    Undo.RecordObject(t, "Inspector");

                    property.SetValue(t, value, null);
                }
            }
        }

        if (serializedProperty != null)
            EditorGUI.EndProperty();
    }

    #endregion

    #region Folder

    [System.Serializable]
    public class FolderSetting
    {
        public bool remove = false;
        public bool duplicate = false;

        public bool directPath = false;
        public string path = "";
        public DefaultAsset folder = null;

        public bool simple = false;

        public FolderSetting() { }

        public FolderSetting(string _path)
        {
            directPath = true;
            path = _path;

            folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
        }

        public FolderSetting(string _path, bool _simple)
        {
            directPath = true;
            path = _path;

            folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);

            simple = _simple;
        }

        public FolderSetting Clone()
        {
            FolderSetting newFolder = new FolderSetting();

            newFolder.directPath = directPath;
            newFolder.path = path;
            newFolder.folder = folder;

            return newFolder;
        }

        public bool OnGUI(int index = 0, bool canRemove = false)
        {
            EditorGUILayout.BeginHorizontal();

            directPath = EditorGUILayout.Toggle("Use direct path", directPath);

            if (!simple)
            {
                EditorGUI.BeginDisabledGroup(remove || !canRemove);
                if (GUILayout.Button("Remove", EditorStyles.miniButton))
                {
                    remove = true;
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(duplicate);
                if (GUILayout.Button("Duplicate", EditorStyles.miniButton))
                {
                    duplicate = true;
                }
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndHorizontal();

            if (directPath)
            {
                EditorGUI.BeginChangeCheck();
                if (!simple)
                {
                    path = EditorGUILayout.TextField($"Folder {index}", path);
                }
                else
                {
                    path = EditorGUILayout.TextField(path);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                if (!simple)
                    folder = EditorGUILayout.ObjectField($"Folder {index}", folder, typeof(DefaultAsset), false) as DefaultAsset;
                else
                    folder = EditorGUILayout.ObjectField(folder, typeof(DefaultAsset), false) as DefaultAsset;

                if (EditorGUI.EndChangeCheck())
                {
                    path = AssetDatabase.GetAssetPath(folder);
                }
            }

            return remove || duplicate;
        }
    }

    [System.Serializable]
    public class FolderList
    {
        public List<string> Defaults { get; } = new List<string>();
        public List<FolderSetting> List { get; set; } = new List<FolderSetting>();

        public string[] Paths
        {
            get
            {
                return List.Select(folder => folder.path).ToArray();
            }
        }

        public bool Showed { get; private set; } = false;
        public string Name { get; private set; } = "Unknown Folders";

        public FolderList(string _name, string[] _defaults)
        {
            Name = _name;
            Defaults.AddRange(_defaults);
        }

        public void AddDefaultsToList()
        {
            if (List.Count == 0)
            {
                List.AddRange(Defaults.Select(path => new FolderSetting(path)));
            }
        }

        public void OnGUI()
        {
            Showed = EditorGUILayout.Foldout(Showed, Name);

            if (Showed)
            {
                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                int newSize = Mathf.Max(EditorGUILayout.IntField("Size", List.Count), 0);

                if (EditorGUI.EndChangeCheck())
                {
                    List.Resize(newSize);
                    AddDefaultsToList();
                }

                for (int folder = 0; folder < List.Count; ++folder)
                {
                    if (List[folder].OnGUI(folder, List.Count > 1))
                    {
                        if (List[folder].remove)
                        {
                            List[folder].remove = false;
                            List.RemoveAt(folder--);
                        }
                        else if (List[folder].duplicate)
                        {
                            List[folder].duplicate = false;
                            List.Insert(folder + 1, List[folder].Clone());
                        }
                    }
                }

                EditorGUI.indentLevel--;
            }
        }
    }

    [System.Serializable]
    public class Loadable<T> where T : Object
    {
        public FolderList FolderList { get; private set; } = null;

        public List<T> list = new List<T>();
        public List<string> names = new List<string>();

        public Loadable(string displayName, string[] _defaultsFolder = null)
        {
            FolderList = new FolderList(displayName, _defaultsFolder ?? new string[0]);
        }

        public delegate Object[] AssetLoadingFunction(string path);

        public int LoadAssets()
        {
            string[] GUIDs = AssetDatabase.FindAssets("t: " + typeof(T).Name, FolderList.Paths).Distinct().ToArray();

            foreach (string guid in GUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                T asset = AssetDatabase.LoadAssetAtPath<T>(path);

                list.Add(asset);

                int nameStartIndex = path.LastIndexOf('/') + 1;
                names.Add(path.Substring(nameStartIndex, path.Length - nameStartIndex));

            }

            return GUIDs.Length;
        }

        public int LoadAssetRepresentations()
        {
            string[] GUIDs = AssetDatabase.FindAssets("t: " + typeof(T).Name, FolderList.Paths).Distinct().ToArray();

            foreach (string guid in GUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

                int nameStartIndex = path.LastIndexOf('/') + 1;
                string name = path.Substring(nameStartIndex, path.Length - nameStartIndex);

                foreach (var asset in subAssets)
                {
                    list.Add(asset as T);

                    names.Add(name);
                }
            }

            return GUIDs.Length;
        }


    }

    #endregion

    #region Drawers

    [CustomPropertyDrawer(typeof(EnforceTypeAttribute))]
    public class PrettyListDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnforceTypeAttribute propAttribute = attribute as EnforceTypeAttribute;
            EditorGUI.BeginProperty(position, label, property);

            MonoBehaviour obj = EditorGUI.ObjectField(position, property.objectReferenceValue, typeof(MonoBehaviour), true) as MonoBehaviour;
            if (obj != null && propAttribute.type.IsAssignableFrom(obj.GetType()) && !EditorGUI.showMixedValue)
            {
                property.objectReferenceValue = obj as MonoBehaviour;
            }
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(TwoLineAttribute))]
    public class TwoLineDrawer : PropertyDrawer
    {
        Vector2 spacing = new Vector2(5f, 2f);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lh = EditorGUIUtility.singleLineHeight;

            return AreaSize(2, lh, spacing.y);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TwoLineAttribute a = attribute as TwoLineAttribute;
            System.Type parentType = GetPropertyParentObject(property).GetType();

            position = EditorGUI.IndentedRect(position);

            Rect[] lines = AreaGridLayout(position, new Vector2Int((int)a.count, 2), spacing);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            for (int i = 0; i < a.count; i++)
            {
                TooltipAttribute tooltip = GetTooltip(parentType.GetField(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), true);

                Rect propRect = lines[i].Merge(lines[i + a.count]);
                label.text = property.displayName;
                label.tooltip = tooltip?.tooltip;

                EditorGUI.BeginProperty(propRect, label, property);

                EditorGUI.PrefixLabel(lines[i], label);
                EditorGUI.PropertyField(lines[i + a.count], property, GUIContent.none);

                EditorGUI.EndProperty();

                property.Next(false);
            }

            EditorGUI.indentLevel = indent;
        }
    }

    [CustomPropertyDrawer(typeof(LockedAttribute))]
    public class LockedDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginDisabledGroup(true);

            EditorGUI.PropertyField(position, property, label, true);

            EditorGUI.EndDisabledGroup();
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(LineAttribute))]
    public class LineDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            LineAttribute a = attribute as LineAttribute;

            return a.paddingAbove + a.thickness + a.paddingUnder;
        }

        public override void OnGUI(Rect position)
        {
            LineAttribute a = attribute as LineAttribute;

            position.y += a.paddingAbove;
            position.height = a.thickness;

            EditorGUI.DrawRect(position, a.color);
        }
    }

    [CustomPropertyDrawer(typeof(VisibleIfAttribute))]
    public class VisibleIfDrawer : PropertyDrawer
    {
        bool isVisible(SerializedProperty property)
        {
            VisibleIfAttribute a = attribute as VisibleIfAttribute;

            object target = property.serializedObject.targetObject;

            FieldInfo field = target.GetType().GetField(a.fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return field.GetValue(target).Equals(a.equalTo) != a.inverted;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (isVisible(property))
            {
                return base.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (isVisible(property))
            {
                EditorGUI.BeginProperty(position, label, property);
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.EndProperty();
            }
        }
    }

    [CustomPropertyDrawer(typeof(OnChangedCallbackAttribute))]
    public class OnChangedCallbackDrawer : PropertyDrawer
    {
        MethodInfo method = null;

        void InvokeCallback(SerializedProperty property)
        {
            OnChangedCallbackAttribute a = attribute as OnChangedCallbackAttribute;

            object target = GetPropertyParentObject(property);

            if (method == null)
                method = target.GetType().GetMethod(a.callbackName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (method == null)
            {
                throw new System.Exception($"OnChangedCallback: Couldn't find the callback method {target.GetType().Name}.{a.callbackName}()\n");
            }

            method.Invoke(target, null);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, true);
            bool changed = EditorGUI.EndChangeCheck();

            EditorGUI.EndProperty();

            if (changed)
            {
                property.serializedObject.ApplyModifiedProperties();

                InvokeCallback(property);
            }
        }
    }

    [CustomPropertyDrawer(typeof(OnChangedAssignAttribute))]
    public class OnChangedAssignDrawer : PropertyDrawer
    {
        MethodInfo method = null;

        void InvokeCallback(SerializedProperty property)
        {
            OnChangedAssignAttribute a = attribute as OnChangedAssignAttribute;

            object target = GetPropertyParentObject(property);

            if (method == null)
                method = target.GetType().GetMethod(a.CallbackName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (method == null)
                throw new System.Exception($"OnChangedAssign: Couldn't find the callback method {target.GetType().Name}.{a.CallbackName}()\n");

            SerializedProperty targetProperty = property.FindSiblingProperty(a.TargetName);

            if (targetProperty == null)
                throw new System.Exception($"OnChangedAssign: Couldn't find the target property {target.GetType().Name}.{a.TargetName}()\n");

            object result = method.Invoke(target, null);

            if (result.GetType() == typeof(void))
                throw new System.Exception($"OnChangedAssign: Callback method returns void {target.GetType().Name}.{a.CallbackName}()\n");

            switch (targetProperty.propertyType)
            {
                case SerializedPropertyType.Integer:
                    targetProperty.intValue = (int)result;
                    break;
                case SerializedPropertyType.Boolean:
                    targetProperty.boolValue = (bool)result;
                    break;
                case SerializedPropertyType.Float:
                    targetProperty.floatValue = (float)result;
                    break;
                case SerializedPropertyType.String:
                    targetProperty.stringValue = (string)result;
                    break;
                case SerializedPropertyType.Color:
                    targetProperty.colorValue = (Color)result;
                    break;
                case SerializedPropertyType.ObjectReference:
                    targetProperty.objectReferenceValue = (Object)result;
                    break;
                case SerializedPropertyType.LayerMask:
                    targetProperty.intValue = (int)result;
                    break;
                case SerializedPropertyType.Enum:
                    targetProperty.enumValueIndex = (int)result;
                    break;
                case SerializedPropertyType.Vector2:
                    targetProperty.vector2Value = (Vector2)result;
                    break;
                case SerializedPropertyType.Vector3:
                    targetProperty.vector3Value = (Vector3)result;
                    break;
                case SerializedPropertyType.Vector4:
                    targetProperty.vector4Value = (Vector4)result;
                    break;
                case SerializedPropertyType.Rect:
                    targetProperty.rectValue = (Rect)result;
                    break;
                case SerializedPropertyType.ArraySize:
                    targetProperty.arraySize = (int)result;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    targetProperty.animationCurveValue = (AnimationCurve)result;
                    break;
                case SerializedPropertyType.Bounds:
                    targetProperty.boundsValue = (Bounds)result;
                    break;
                case SerializedPropertyType.Quaternion:
                    targetProperty.quaternionValue = (Quaternion)result;
                    break;
                case SerializedPropertyType.ExposedReference:
                    targetProperty.exposedReferenceValue = (Object)result;
                    break;
                case SerializedPropertyType.Vector2Int:
                    targetProperty.vector2IntValue = (Vector2Int)result;
                    break;
                case SerializedPropertyType.Vector3Int:
                    targetProperty.vector3IntValue = (Vector3Int)result;
                    break;
                case SerializedPropertyType.RectInt:
                    targetProperty.rectIntValue = (RectInt)result;
                    break;
                case SerializedPropertyType.BoundsInt:
                    targetProperty.boundsIntValue = (BoundsInt)result;
                    break;
                case SerializedPropertyType.ManagedReference:
                    targetProperty.managedReferenceValue = System.Convert.ChangeType(result, System.Type.GetType(targetProperty.managedReferenceFullTypename));
                    break;
                default:
                    throw new System.Exception($"OnChangedAssign: Unsupported property type {targetProperty.propertyType} {target.GetType().Name}.{a.TargetName}()\n");
        }

            property.serializedObject.ApplyModifiedProperties();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, true);
            bool changed = EditorGUI.EndChangeCheck();

            EditorGUI.EndProperty();

            if (changed)
            {
                property.serializedObject.ApplyModifiedProperties();

                InvokeCallback(property);
            }
        }
    }

    [CustomPropertyDrawer(typeof(SerializePropertyAttribute))]
    public class SerializePropertyAttributeDrawer : PropertyDrawer
    {
        private PropertyInfo propertyFieldInfo = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Object[] targets = property.serializedObject.targetObjects;

            if (propertyFieldInfo == null)
                propertyFieldInfo = targets[0].GetType().GetProperty(((SerializePropertyAttribute)attribute).PropertyName,
                                                         BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            PropertyField(targets, propertyFieldInfo, property.propertyType, property);
        }
    }

    [CustomPropertyDrawer(typeof(GroupAttribute))]
    public class GroupAttributeDrawer : PropertyDrawer
    {
        bool expanded = true;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty cur = property.Copy();

            float height = EditorGUIUtility.singleLineHeight;

            if (expanded)
            {
                for (int i = 0; i < (attribute as GroupAttribute).PropertyCount; i++)
                {
                    height += EditorGUI.GetPropertyHeight(cur, true);

                    if (!cur.Next(false)) return height;
                }
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty cur = property.Copy();

            float lh = EditorGUIUtility.singleLineHeight;
            float vs = EditorGUIUtility.standardVerticalSpacing;

            Rect[] zones = AreaCutoff(position, Side.Up, lh, vs);
            Rect rTitle = zones[0];
            Rect rProperties = zones[1];

            expanded = EditorGUI.Foldout(rTitle, expanded, (attribute as GroupAttribute).GroupName);
            
            if (expanded)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < (attribute as GroupAttribute).PropertyCount; i++)
                {
                    float height = EditorGUI.GetPropertyHeight(cur, true);

                    zones = AreaCutoff(rProperties, Side.Up, height, 0f);
                    Rect rCur = zones[0];
                    rProperties = zones[1];

                    EditorGUI.PropertyField(rCur, cur, true);

                    if (!cur.Next(false)) return;
                }

                EditorGUI.indentLevel--;
            }
        }
    }

    [CustomPropertyDrawer(typeof(RangeFloat))]
    public class RangeFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.IndentedRect(position);

            float labelWidth = EditorGUIUtility.labelWidth;
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            SerializedProperty min = property.FindPropertyRelative("min");
            SerializedProperty max = property.FindPropertyRelative("max");

            EditorGUI.BeginProperty(position, label, property);

            // Label
            position = EditorGUI.PrefixLabel(position, label);

            Rect[] line = AreaGridLayout(position, new Vector2Int(2, 1), new Vector2(5f, 5f));
            EditorGUIUtility.labelWidth = 25f;

            // Min
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(line[0], min);
            if (EditorGUI.EndChangeCheck())
            {
                max.floatValue = Mathf.Max(min.floatValue, max.floatValue);
            }

            // Max
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(line[1], max);
            if (EditorGUI.EndChangeCheck())
            {
                min.floatValue = Mathf.Min(min.floatValue, max.floatValue);
            }

            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(RangeInt))]
    public class RangeIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.IndentedRect(position);

            float labelWidth = EditorGUIUtility.labelWidth;
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            SerializedProperty min = property.FindPropertyRelative("min");
            SerializedProperty max = property.FindPropertyRelative("max");

            EditorGUI.BeginProperty(position, label, property);

            // Label
            position = EditorGUI.PrefixLabel(position, label);

            Rect[] line = AreaGridLayout(position, new Vector2Int(2, 1), new Vector2(5f, 5f));
            EditorGUIUtility.labelWidth = 25f;

            // Min
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(line[0], min);
            if (EditorGUI.EndChangeCheck())
            {
                max.intValue = Mathf.Max(min.intValue, max.intValue);
            }

            // Max
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(line[1], max);
            if (EditorGUI.EndChangeCheck())
            {
                min.intValue = Mathf.Min(min.intValue, max.intValue);
            }

            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(ClampedFloat))]
    public class ClampedFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty val = property.FindPropertyRelative("val");
            SerializedProperty min = property.FindPropertyRelative("min");
            SerializedProperty max = property.FindPropertyRelative("max");

            EditorGUI.BeginProperty(position, label, property);

            // Label
            position = EditorGUI.PrefixLabel(position, label);

            // Fields
            float labelWidth = EditorGUIUtility.labelWidth;
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect[] line = AreaGridLayout(position, new Vector2Int(3, 1), new Vector2(5f, 5f));
            EditorGUIUtility.labelWidth = 25f;

            bool checkVal = false;

            // Min
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(line[1], min);
            if (EditorGUI.EndChangeCheck())
            {
                checkVal = true;
                max.floatValue = Mathf.Max(min.floatValue, max.floatValue);
            }

            // Max
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(line[2], max);
            if (EditorGUI.EndChangeCheck())
            {
                checkVal = true;
                min.floatValue = Mathf.Min(min.floatValue, max.floatValue);
            }

            // Value
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(line[0], val);
            checkVal = checkVal || EditorGUI.EndChangeCheck();
            if (checkVal)
            {
                checkVal = false;
                val.floatValue = Mathf.Clamp(val.floatValue, min.floatValue, max.floatValue);
            }

            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(ClampedInt))]
    public class ClampedIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty val = property.FindPropertyRelative("val");
            SerializedProperty min = property.FindPropertyRelative("min");
            SerializedProperty max = property.FindPropertyRelative("max");

            EditorGUI.BeginProperty(position, label, property);

            // Label
            position = EditorGUI.PrefixLabel(position, label);

            // Fields
            float labelWidth = EditorGUIUtility.labelWidth;
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect[] line = AreaGridLayout(position, new Vector2Int(3, 1), new Vector2(5f, 5f));
            EditorGUIUtility.labelWidth = 25f;

            bool checkVal = false;

            // Min
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(line[1], min);
            if (EditorGUI.EndChangeCheck())
            {
                checkVal = true;
                max.intValue = Mathf.Max(min.intValue, max.intValue);
            }

            // Max
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(line[2], max);
            if (EditorGUI.EndChangeCheck())
            {
                checkVal = true;
                min.intValue = Mathf.Min(min.intValue, max.intValue);
            }

            // Value
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(line[0], val);
            checkVal = checkVal || EditorGUI.EndChangeCheck();
            if (checkVal)
            {
                checkVal = false;
                val.intValue = Mathf.Clamp(val.intValue, min.intValue, max.intValue);
            }

            EditorGUIUtility.labelWidth = labelWidth;

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(TimerBuffer))]
    [CustomPropertyDrawer(typeof(Timer))]
    public class TimerDrawer : PropertyDrawer
    {
        float progressSpacing = 2f;
        float progressHeight = 2f;

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return EditorApplication.isPlaying;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float lh = EditorGUIUtility.singleLineHeight;

            return lh + (EditorApplication.isPlaying ? progressHeight + progressSpacing : 0f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float lh = EditorGUIUtility.singleLineHeight;
            position.height = lh;

            if (!EditorApplication.isPlaying)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("duration"), label);
            }
            else
            {
                Rect[] zones = AreaCutoff(position, Side.Right, lh, EditorGUIUtility.standardVerticalSpacing);

                EditorGUI.PropertyField(zones[0], property.FindPropertyRelative("duration"), label);

                Timer timer = (Timer)GetTargetObjectWithProperty(property);

                if (GUI.Button(zones[1], "►"))
                {
                    timer.Start();
                }

                position.y += position.height + progressSpacing;
                position.height = progressHeight;

                if (timer.IsStarted)
                {
                    EditorGUI.DrawRect(position, Color.black);

                    position.width *= timer.Progress;
                    EditorGUI.DrawRect(position, Color.green);
                }
                else
                {
                    EditorGUI.DrawRect(position, Color.red);
                }
            }

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(SceneReference))]
    public class ScreneReferenceDrawer : PropertyDrawer
    {
        GUIStyle infoStyle = null;

        const string msgSceneNotFound = "Invalid scene name (is the scene in the build list ?)";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty buildIndex = property.FindPropertyRelative("buildIndex");

            float baseHeight = base.GetPropertyHeight(property, label);
            float infoHeight = 0f;

            if (buildIndex.intValue == -1)
            {
                CreateInfoStyle();

                infoHeight = infoStyle.CalcHeight(new GUIContent(msgSceneNotFound), EditorGUIUtility.currentViewWidth);
            }

            return baseHeight + infoHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CreateInfoStyle();

            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;

            Rect baseRect = position;
            float fieldWidth = position.width - EditorGUIUtility.labelWidth;
            float indexWidth = 20f;

            SerializedProperty name = property.FindPropertyRelative("name");
            SerializedProperty buildIndex = property.FindPropertyRelative("buildIndex");

            // Label
            position = EditorGUI.PrefixLabel(position, label, new GUIStyle() { richText = true });

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Name
            EditorGUI.BeginChangeCheck();
            position.width = fieldWidth - indexWidth;
            EditorGUI.PropertyField(position, name, GUIContent.none);
            position.x += position.width;

            if (EditorGUI.EndChangeCheck())
            {
                buildIndex.intValue = -1;

                for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                {
                    string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

                    int nameStart = scenePath.LastIndexOf('/') + 1;
                    int nameEnd = scenePath.LastIndexOf(".unity");

                    if (scenePath.Substring(nameStart, nameEnd - nameStart) == name.stringValue)
                    {
                        buildIndex.intValue = i;
                        break;
                    }
                }

                property.serializedObject.ApplyModifiedProperties();
            }

            // Build index
            position.width = indexWidth;

            EditorGUI.LabelField(position, buildIndex.intValue.ToString());

            // Invalid scene
            if (buildIndex.intValue == -1)
            {
                position = baseRect;
                position.y += EditorGUIUtility.singleLineHeight;
                position.height = infoStyle.CalcHeight(new GUIContent(msgSceneNotFound), position.width);

                EditorGUI.LabelField(position, msgSceneNotFound, infoStyle);
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        void CreateInfoStyle()
        {
            if (infoStyle == null)
            {
                infoStyle = new GUIStyle();

                infoStyle.normal.background = AssetDatabase.GetBuiltinExtraResource<Texture2D>("UI/Skin/Background.psd");
                infoStyle.normal.textColor = Color.red;
                infoStyle.fontStyle = FontStyle.Bold;
                infoStyle.border = new RectOffset(10, 10, 10, 10);
                infoStyle.padding = new RectOffset(10, 10, (int)(.5f * EditorGUIUtility.singleLineHeight), (int)(.5f * EditorGUIUtility.singleLineHeight));
                infoStyle.wordWrap = true;
                infoStyle.stretchHeight = true;
            }
        }
    }

    #endregion

#endif
}
