using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace _VictorDev.TCIT.DCIM
{
    /// 機櫃/設備 COBie資訊
    [Serializable]
    public class Information
    {
        public float height;
        public float heightU;
        public float watt;
        public float weight;
        public string component_description;
        public string component_assetIdentifier;
        public string component_installationDate;
        public string component_tagName;
        public string component_warrantyDurationPart;
        public string component_warrantyDurationUnit;
        public string component_warrantyGuarantorLabor;
        public string component_warrantyEndDate;
        public string component_serialNumber;
        public string component_warrantyStartDate;
        public string contact_company;
        public string contact_department;
        public string contact_email;
        public string contact_familyName;
        public string contact_givenName;
        public string contact_phone;
        public string contact_street;
        public string facility_name;
        public string facility_projectName;
        public string facility_siteName;
        public string floor_name;
        public string space_name;
        public string space_roomTag;
        public string system_category;
        public string system_name;
        public string type_category;
        public string type_expectedLife;
        public string type_manufacturer;
        public string type_modelNumber;
        public string type_name;
        public string type_accessibilityPerformance;
        public string type_replacementCost;
        public string type_shape;
        public string type_size;
        public string type_color;
        public string type_finish;
        public string type_grade;
        public string type_material;
        public string document_inspection;
        public string document_handout;
        public string document_drawing;
        public string equipment_supplier;
    }
}