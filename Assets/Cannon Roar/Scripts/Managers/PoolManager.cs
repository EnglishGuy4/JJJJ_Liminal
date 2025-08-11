using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{

    public static PoolManager current;
    public GameObject[] collectionOfObjectsToBePooled;      //Collection of object to be pooled
    public int[] pooledAmountForEachObject;                 //Number of objects to be pooled
    public bool willGrow = true;                            //If this variable is set to true it will allow the pooler to create a new object if one is not avaliable in the pool

    private Dictionary<string, List<GameObject>> poolerData = new Dictionary<string, List<GameObject>>();

    public void Awake()
    {
        current = this;
    }

    // Use this for initialization of the objects in the collectionOfObjectsToBePooled array 
    public void Start()
    {
        //loops through the collectionOfObjectsToBePooled pooled array
        for (int i = 0; i < collectionOfObjectsToBePooled.Length; i++)
        {
            List<GameObject> pooledObjects;
            pooledObjects = new List<GameObject>();
            //loops through the pooledAmountForEachObject pooled array
            for (int j = 0; j < pooledAmountForEachObject[i]; j++)
            {
                GameObject obj = Instantiate(collectionOfObjectsToBePooled[i], transform); //Creates the object in the scene
                obj.SetActive(false);
                pooledObjects.Add(obj); //Adds the newly created object to an array so it can be stored in a Dictionary
            }
            //Adds the list of the same objects that are stored in the pooledObjects array to a Dictionary under the name of the oject.
            poolerData.Add(collectionOfObjectsToBePooled[i].name, pooledObjects);
        }
    }

    //This function takes the name of the object you would like to activate
    //It then searches for a non active version of the object and returns it
    public GameObject GetPooledObject(string nameOfPooledObject)
    {
        //nameOfPooledObject holds the name of the object you would like to access in the dictionary
        //poolerData[nameOfPooledObject].Count // Loads the array stored under the specifed dictionary key and counts how many elements are in the array
        for (int i = 0; i < poolerData[nameOfPooledObject].Count; i++)
        {
            //Searches the array for the first non active game object
            if (!poolerData[nameOfPooledObject][i].activeInHierarchy)
            {
                return poolerData[nameOfPooledObject][i]; //Returns the first nonactive game object
            }
        }
        //If a non active version of an object is not available a new one is created and added to the Dictionary for future use
        if (willGrow)
        {
            GameObject obj = Instantiate(poolerData[nameOfPooledObject][0], transform);
            obj.transform.position = new Vector3(0, 0, 0);
            obj.SetActive(false);
            poolerData[nameOfPooledObject].Add(obj);
            return obj;
        }
        return null;
    }

}
