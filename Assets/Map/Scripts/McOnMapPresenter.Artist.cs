using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public partial class McOnMapPresenter
{
	public class McAddress
	{
		public string folder_name;
		public string prefab_name;

		public McAddress(string folder_name, string prefab_name)
		{
			this.folder_name = folder_name;
			this.prefab_name = prefab_name;
		}
	}

	//Mapping <BuildingId, Character_prefab_address>
	public static Dictionary<(string building_id, int order_in_building), McAddress> AddressMapping = new Dictionary<(string building_id, int order_in_building), McAddress>()
	{
		 { ("building_basic_01", 0) , new McAddress("B01_Luna", "mc-luna") },
		 { ("building_basic_02", 0) , new McAddress("B02_Fisher Man", "mc-fisher-man") },
		 { ("building_basic_03", 0) , new McAddress("B03_Sushi Chef", "mc-sushi-chef") },
		 { ("building_basic_04", 0) , new McAddress("B04_Carpenter", "mc-carpenter") },
		 { ("building_basic_05", 0) , new McAddress("B05_Clam Raker", "mc-clam-raker") },
		 //{ ("building_basic_06", 0) , new McAddress("", "") },
		 { ("building_basic_07", 0) , new McAddress("B07_Blacksmith", "mc-blacksmith") },
		 //{ ("building_basic_08", 0) , new McAddress("", "") },
		 { ("building_basic_09", 0) , new McAddress("B09_TheGuard", "mc-the-guard") },
		 { ("building_basic_10", 0) , new McAddress("B10_Monk Cat", "mc-monk-cat") },
		 { ("building_basic_11", 0) , new McAddress("B11_Wheat Farmer", "mc-whear-farmer") },
		 { ("building_basic_12", 0) , new McAddress("B12_Flour Packer", "mc-flour-packer") },
		 { ("building_basic_13", 0) , new McAddress("B13_Baker", "mc-baker") },
		 { ("building_basic_14", 0) , new McAddress("B14_Coffee Farmer", "mc-coffee-farmer") },
		 { ("building_basic_15", 0) , new McAddress("B15_The Waiter", "mc-the-waiter") },
		 { ("building_basic_16", 0) , new McAddress("B16_Apiarist", "mc-apiarist") },
		 { ("building_basic_17", 0) , new McAddress("B17_Merchant_1", "mc-merchant-1") },
		 { ("building_basic_17", 1) , new McAddress("B17_Merchant_2", "mc-merchant-2") },
		 { ("building_basic_17", 2) , new McAddress("B17_Merchant_3", "mc-merchant-3") },
		 { ("building_basic_18", 0) , new McAddress("B18_ShipwrightSailor", "mc-shipwright-sailor") },
		 //{ ("building_basic_19", 0) , new McAddress("", "") },
		 { ("building_basic_20", 0) , new McAddress("B20_Traveler", "mc-traveler") },
		 { ("building_basic_21", 0) , new McAddress("B21_Cat in bath", "mc-cat-in-bath") },
		 { ("building_basic_22", 0) , new McAddress("B22_Strawberry Farmer", "mc-strawberry-farmer") },
		 { ("building_basic_23", 0) , new McAddress("B23_Cacao Farmer", "mc-cacao-farmer") },
		 { ("building_basic_24", 0) , new McAddress("B24_Seller", "mc-seller") },
		 { ("building_basic_25", 0) , new McAddress("B25_Touris 1", "mc-tourist-1") },
		 { ("building_basic_26", 0) , new McAddress("B26_BeachLifeguard", "mc-beach-lifeguard") },
		 { ("building_basic_27", 0) , new McAddress("B27_Touris 2", "mc-tourist-2") },
		 //{ ("building_basic_28", 0) , new McAddress("", "") },
		 { ("building_basic_29", 0) , new McAddress("B29_Doctor", "mc-doctor") },
		 { ("building_basic_30", 0) , new McAddress("B30_FireFighter", "mc-fire-fighter") },
		 { ("building_basic_31", 0) , new McAddress("B31_Policecat", "mc-police-cat") },
	};

	//Load MC Prefab Editor

	public string GetPrefabPath_Editor()
	{
		if (AddressMapping.ContainsKey((buildingId, orderInBuilding)))
		{
			var address = AddressMapping[(buildingId, orderInBuilding)];
			if (address == null || string.IsNullOrEmpty(address.folder_name) || string.IsNullOrEmpty(address.prefab_name))
			{
				Debug.LogWarning($"McOnMapPresenter > GetPrefabPath_Editor > AddressMapping for BuildingId {buildingId} OrderInBuilding {orderInBuilding} is invalid!");
				return null;
            }
			return $"{address.folder_name}/prefabs/{address.prefab_name}";
        }
		return null;
	}
}
#endif