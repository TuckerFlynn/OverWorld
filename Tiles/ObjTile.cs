using System;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Object Tile", menuName = "Tiles/Object Tile")]
    public class ObjTile : TileBase
    {
        [SerializeField]
        public Sprite DefaultSprite;
        public string group;
        public GameObject DefaultGameObject;
        public Tile.ColliderType DefaultColliderType = Tile.ColliderType.None;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = DefaultSprite;
            tileData.gameObject = DefaultGameObject;
            tileData.colliderType = DefaultColliderType;
            //base.GetTileData(position, tilemap, ref tileData);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ObjTile))]
    public class ObjTileEditor : Editor
    {
        private ObjTile tile { get { return (target as ObjTile); } }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            tile.DefaultSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", tile.DefaultSprite, typeof(Sprite), false, null);
            tile.group = EditorGUILayout.TextField("Group", "");
            tile.DefaultGameObject = (GameObject)EditorGUILayout.ObjectField("GameObject", tile.DefaultGameObject, typeof(GameObject), false, null);
            tile.DefaultColliderType = (Tile.ColliderType)EditorGUILayout.EnumPopup("Default Collider", tile.DefaultColliderType);

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(tile);
        }

        // Shows the Tile sprite as the asset thumbnail
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            if (tile.DefaultSprite != null)
            {
                Type t = GetType("UnityEditor.SpriteUtility");
                if (t != null)
                {
                    MethodInfo method = t.GetMethod("RenderStaticPreview", new Type[] { typeof(Sprite), typeof(Color), typeof(int), typeof(int) });
                    if (method != null)
                    {
                        object ret = method.Invoke("RenderStaticPreview", new object[] { tile.DefaultSprite, Color.white, width, height });
                        if (ret is Texture2D)
                            return ret as Texture2D;
                    }
                }
            }
            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }
        // Taken from RuleTileEditor.cs
        public static Type GetType(string TypeName)
        {
            var type = Type.GetType(TypeName);
            if (type != null)
                return type;

            if (TypeName.Contains("."))
            {
                var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));
                var assembly = Assembly.Load(assemblyName);
                if (assembly == null)
                    return null;
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }

            var currentAssembly = Assembly.GetExecutingAssembly();
            var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssemblies)
            {
                var assembly = Assembly.Load(assemblyName);
                if (assembly != null)
                {
                    type = assembly.GetType(TypeName);
                    if (type != null)
                        return type;
                }
            }
            return null;
        }
    }
#endif
}