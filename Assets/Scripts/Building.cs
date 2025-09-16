using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour
{
    [Header("�ǹ� ����")]
    public BuildingType BuildingType;
    public string buildingName = "�ǹ�";

    [System.Serializable]

    public class BuildingEvents
    {
        public UnityEvent<string> OnDriverEntered;
        public UnityEvent<string> OnDriverExited;
        public UnityEvent<BuildingType> OnserviceUsed;
    }

    public BuildingEvents buildingEvents;

    private DeliveryOrderSystem orderSystem;


    void Start()
    {
        SetupBuilding();
        orderSystem = FindAnyObjectByType<DeliveryOrderSystem>();
        CreateNameTag();
    }


    void SetupBuilding()
    {
        Renderer randerer = GetComponent<Renderer>();
        if (randerer != null)
        {
            Material mat = randerer.material;
            switch (BuildingType)
            {
                case BuildingType.Restaurant:
                    mat.color = Color.red;
                    break;

                case BuildingType.Customer:
                    mat.color = Color.green;
                    break;

                case BuildingType.ChargingStation:
                    mat.color = Color.yellow;
                    break;

            }
        }
        Collider col = GetComponent<Collider>();
        if (col != null) { col.isTrigger = true; }

    }

    void OnTriggerEnter(Collider other)
    {
        DeliveryDriver driver = other.GetComponent<DeliveryDriver>();
        if (driver != null)
        {
            buildingEvents.OnDriverEntered?.Invoke(buildingName);
            HandleDriverService(driver);
        }
    }

    void OnTriggerExit(Collider other)
    {
        DeliveryDriver driver = other.GetComponent<DeliveryDriver>();
        if (driver != null)
        {
            buildingEvents.OnDriverExited?.Invoke(buildingName);
            Debug.Log($"{buildingName} �� �������ϴ� ");
        }
    }

    void CreateNameTag()
    {
        //�ǹ� ���� �̸�ǥ ����
        GameObject nameTag = new GameObject("NameTag");
        nameTag.transform.SetParent(transform);
        nameTag.transform.localPosition = Vector3.up * 1.5f;

        TextMesh textMesh = nameTag.AddComponent<TextMesh>();
        textMesh.text = buildingName;
        textMesh.characterSize = 0.2f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.white;
        textMesh.fontSize = 20;

        nameTag.AddComponent<Building>();
    }

    void HandleDriverService(DeliveryDriver driver)
    {
        switch (BuildingType)
        {
            case BuildingType.Restaurant:
                if (orderSystem != null)
                {
                    orderSystem.OnDriverEnteredRestaurant(this);

                }
                break;
            case BuildingType.Customer:
                if (orderSystem != null)
                {
                    orderSystem.OnDriverEnteredCustom(this);

                }
                else
                {
                    driver.CompleteDelivery();

                }

                break;

            case BuildingType.ChargingStation:


                driver.ChargeBattery();
                break;

        }

        buildingEvents.OnserviceUsed?.Invoke(BuildingType);
    }
}




