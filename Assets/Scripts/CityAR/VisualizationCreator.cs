using DefaultNamespace;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace CityAR
{
    public class VisualizationCreator : MonoBehaviour
    {
        int countBuildings=0;
        public GameObject districtPrefab;
        public GameObject buildingPrefab;
        private DataObject _dataObject;
        private GameObject _platform;
        private Data _data;

        private void Start()
        {
            _platform = GameObject.Find("Platform");
            _data = _platform.GetComponent<Data>();
            _dataObject = _data.ParseData();
            BuildCity(_dataObject);
        }

        private void BuildCity(DataObject p)
        {
            if (p.project.files.Count > 0)
            {
                p.project.w = 1;
                p.project.h = 1;
                p.project.deepth = 1;
                BuildDistrict(p.project, false);
            }
        }

        /*
         * entry: Single entry from the data set. This can be either a folder or a single file.
         * splitHorizontal: Specifies whether the subsequent children should be split horizontally or vertically along the parent
         */
        private void BuildDistrict(Entry entry, bool splitHorizontal)
        {
            if (entry.type.Equals("File"))
            {
                if(countBuildings>=0){
                    BuildBuilding(entry);
                }
                return;

            }
            
            float x = entry.x;
            float z = entry.z;

            float dirLocs = entry.numberOfLines;

            entry.color = GetColorForDepth(entry.deepth);

            BuildDistrictBlock(entry, false);

            foreach (Entry subEntry in entry.files) {
                subEntry.x = x;
                subEntry.z = z;
                
                if (subEntry.type.Equals("Dir"))
                {
                    float ratio = subEntry.numberOfLines / dirLocs;
                    subEntry.deepth = entry.deepth + 1;

                    if (splitHorizontal) {
                        subEntry.w = ratio * entry.w; // split along horizontal axis
                        subEntry.h = entry.h;
                        x += subEntry.w;
                    } else {
                        subEntry.w = entry.w;
                        subEntry.h = ratio * entry.h; // split along vertical axis
                        z += subEntry.h;
                    }
                }
                else
                {
                    subEntry.parentEntry = entry;
                }
                BuildDistrict(subEntry, !splitHorizontal);
                
            }

            //hier kommen nur die Entry an die keine Files sind sondern Ordner
            //jeder Ordner wird auch nochmal eine Basis. Oben wurden die Subentry Kinder auf die ohne Basis gesetzt?

            if (!splitHorizontal)
            {
                entry.x = x;
                entry.z = z;
                if (ContainsDirs(entry))
                {
                    entry.h = 1f - z;
                }
                
            }
            else
            {
                entry.x = -x;
                entry.z = z;
                if (ContainsDirs(entry))
                {
                    entry.w = 1f - x;
                }
            }
            entry.deepth += 1;
            BuildDistrictBlock(entry, true); 
            
            //Hier wird nochmal eine gruene Plane zum Bereits vorhandenen nicht-basis eintrag erstellt, Order mit Ordner und Files drin
            //die Files stehen aber nicht auf der Basis-Plane sondern der Nicht-Basis Plane -> schlecht.
        }

        /*
         * entry: Single entry from the data set. This can be either a folder or a single file.
         * isBase: If true, the entry has no further subfolders. Buildings must be placed on top of the entry
         */
        private void BuildDistrictBlock(Entry entry, bool isBase)
        {
            if (entry == null)
            {
                return;
            }
            
            float w = entry.w; // w -> x coordinate
            float h = entry.h; // h -> z coordinate
            
            if (w * h > 0)
            {
                GameObject prefabInstance;
                if(entry.parentEntry!=null){
                    var goc=entry.parentEntry.goc;
                    var goc_gameObject=goc.gameObject;
                    var trans=goc_gameObject.transform;
                    prefabInstance = Instantiate(districtPrefab, trans, true);
                }
                else{
                    prefabInstance = Instantiate(districtPrefab, _platform.transform, true);
                }
                entry.goc=prefabInstance.transform.GetChild(0).gameObject.GetComponent<GridObjectCollection>();

                if (!isBase)
                {
                    prefabInstance.name = entry.name;
                    prefabInstance.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = entry.color;
                    prefabInstance.transform.localScale = new Vector3(entry.w, 1f,entry.h);
                    prefabInstance.transform.localPosition = new Vector3(entry.x, entry.deepth, entry.z);
                }
                else
                {
                    prefabInstance.name = entry.name+"Base";
                    prefabInstance.transform.GetChild(0).rotation = Quaternion.Euler(90,0,0);
                    prefabInstance.transform.localScale = new Vector3(entry.w, 1,entry.h);
                    prefabInstance.transform.localPosition = new Vector3(entry.x, entry.deepth+0.001f, entry.z);
                    
                    
                }
                
                Vector3 scale = prefabInstance.transform.localScale;
                float scaleX = scale.x - (entry.deepth * 0.005f);
                float scaleZ = scale.z - (entry.deepth * 0.005f);
                float shiftX = (scale.x - scaleX) / 2f;
                float shiftZ = (scale.z - scaleZ) / 2f;
                prefabInstance.transform.localScale = new Vector3(scaleX, scale.y, scaleZ);
                Vector3 position = prefabInstance.transform.localPosition;
                prefabInstance.transform.localPosition = new Vector3(position.x - shiftX, position.y, position.z + shiftZ);
                entry.goc.UpdateCollection();
                
            }
        }



        private void BuildBuilding(Entry entry)
        {
            if (entry == null || (entry.parentEntry.name.Contains("Base")))
            {
                return;
            }
            float dirLocs = entry.parentEntry.numberOfLines;
            float ratio = entry.numberOfLines / dirLocs;
            float d=entry.numberOfLines;
            entry.w=ratio;
            entry.h=ratio;
            var goc=entry.parentEntry.goc;
            var goc_gameObject=goc.gameObject;
            var trans=goc_gameObject.transform;

            if (entry.w * entry.h > 0)
            {

                GameObject prefabInstance = Instantiate(buildingPrefab, trans , true);
                prefabInstance.name = entry.name;
                prefabInstance.transform.localPosition=new Vector3(0f,0f,0f);
                prefabInstance.transform.localScale = new Vector3(entry.w, d ,entry.h);
                goc.UpdateCollection();
            }

            countBuildings++;
        }

        private bool ContainsDirs(Entry entry)
        {
            foreach (Entry e in entry.files)
            {
                if (e.type.Equals("Dir"))
                {
                    return true;
                }
            }

            return false;
        }
        
        private Color GetColorForDepth(int depth)
        {
            Color color;
            switch (depth)
            {
                case 1:
                    color = new Color(179f / 255f, 209f / 255f, 255f / 255f);
                    break;
                case 2:
                    color = new Color(128f / 255f, 179f / 255f, 255f / 255f);
                    break;
                case 3:
                    color = new Color(77f / 255f, 148f / 255f, 255f / 255f);
                    break;
                case 4:
                    color = new Color(26f / 255f, 117f / 255f, 255f / 255f);
                    break;
                case 5:
                    color = new Color(0f / 255f, 92f / 255f, 230f / 255f);
                    break;
                default:
                    color = new Color(0f / 255f, 71f / 255f, 179f / 255f);
                    break;
            }

            return color;
        }
    }
}