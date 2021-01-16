using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using System.IO;

public class DataManager : Utility.UniqueInstance<DataManager>
{

    public List<ItemData> items = new List<ItemData>();
    public List<string> vehicles = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadData());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public ItemData GetItem(int id)
    {
        return items.Find(item => item.id == id);
    }

    public ItemData GetItem(string itemName)
    {
        return items.Find(item => item.title == itemName);
    }

    public IEnumerator LoadData()
    {
        int index = 0;
        // ICONS

        yield return Addressables.LoadAssetsAsync<ItemData>("ItemData", op =>
        {
            op.id = index++;
            items.Add(op);
            MenuManager.Instance.inventory.AddItem(op);
        });

        List<string> vehicles = new List<string>(Directory.GetFiles(Application.dataPath, "*.vehicle"));

        foreach (var item in vehicles)
        {
            int start = item.LastIndexOf('\\') + 1;
            string _name = item.Substring(start, item.LastIndexOf('.') - start);

            MenuManager.Instance.prefabs.AddButton(_name);
        }
    }

    public void SaveVehicle(Vehicle _vehicle, string _name)
    {
        using (FileStream fileStream = new FileStream(Application.dataPath + "/" + _name + ".vehicle", FileMode.Create))
        using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
        {
            binaryWriter.Write(_vehicle.rbs.Count);

            foreach (Rigidbody rb in _vehicle.rbs)
            {
                Block block = rb.transform.GetComponent<Block>();
                block.Write(binaryWriter);
            }

            MenuManager.Instance.prefabs.AddButton(_name);
        }

    }

    public void CreatePrefab(string name)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        int layer = 1 << LayerMask.NameToLayer("Default");


        using (FileStream fileStream = new FileStream(Application.dataPath + "/" + name + ".vehicle", FileMode.Open))
        using (BinaryReader binaryReader = new BinaryReader(fileStream))
        {
            if (Physics.Raycast(ray, out hit, 7f, layer))
            {
                GameObject parent = new GameObject("Vehicle");
                Vehicle vehicle = parent.AddComponent<Vehicle>();
                parent.transform.position = hit.point;

                int nb = binaryReader.ReadInt32();

                for (int i = 0; i < nb; i++)
                {
                    string blockName = binaryReader.ReadString();
                    GameObject go = Instantiate(GetItem(blockName).model, parent.transform);
                    go.name = blockName;
                    Block newBlock = go.GetComponent<Block>();
                    Rigidbody rb = go.GetComponent<Rigidbody>();
                    vehicle.rbs.Add(rb);
                    newBlock.Read(binaryReader);
                }

                StartCoroutine(AttachDelay(vehicle));
            }
        }
    }

    public IEnumerator AttachDelay(Vehicle vehicle)
    {
        yield return new WaitForSeconds(1f);

        for (int i = vehicle.rbs.Count - 1; i >= 0; i--)
        {
            Block newBlock = vehicle.rbs[i].transform.GetComponent<Block>();
            newBlock.Attach();
        }
    }
}