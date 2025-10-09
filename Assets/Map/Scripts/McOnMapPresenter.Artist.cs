using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public partial class McOnMapPresenter
{
	//Mapping <BuildingId, Character_prefab_address>
	public static Dictionary<(string building_id, int order_in_building), string> AddressMapping = new Dictionary<(string building_id, int order_in_building), string>()
	{
		 { ("building_basic_01", 0) , "B01_Luna/prefabs/mc-luna" },
		 { ("building_basic_02", 0) , "B02_Fisher Man/prefabs/mc-fisher-man" },
		 { ("building_basic_03", 0) , "B03_Sushi Chef/prefabs/mc-sushi-chef" },
		 { ("building_basic_04", 0) , "B04_Carpenter/prefabs/mc-carpenter" },
		 { ("building_basic_05", 0) , "B05_Clam Raker/prefabs/mc-clam-raker" },
		 //{ ("building_basic_06", 0) , "" },
		 { ("building_basic_07", 0) , "B07_Blacksmith/prefabs/mc-blacksmith" },
		 //{ ("building_basic_08", 0) , "" },
		 { ("building_basic_09", 0) , "B09_TheGuard/prefabs/mc-the-guard" },
		 { ("building_basic_10", 0) , "B10_Monk Cat/prefabs/mc-monk-cat" },
		 { ("building_basic_11", 0) , "B11_Wheat Farmer/prefabs/mc-whear-farmer" },
		 { ("building_basic_12", 0) , "B12_Flour Packer/prefabs/mc-flour-packer" },
		 { ("building_basic_13", 0) , "B13_Baker/prefabs/mc-baker" },
		 { ("building_basic_14", 0) , "B14_Coffee Farmer/prefabs/mc-coffee-farmer" },
		 { ("building_basic_15", 0) , "B15_The Waiter/prefabs/mc-the-waiter" },
		 { ("building_basic_16", 0) , "B16_Apiarist/prefabs/mc-apiarist" },
		 { ("building_basic_17", 0) , "B17_Merchant_1/prefabs/mc-merchant-1" },
		 { ("building_basic_17", 1) , "B17_Merchant_2/prefabs/mc-merchant-2" },
		 { ("building_basic_17", 2) , "B17_Merchant_3/prefabs/mc-merchant-3" },
		 { ("building_basic_18", 0) , "B18_ShipwrightSailor/prefabs/mc-shipwright-sailor" },
		 //{ ("building_basic_19", 0) , "" },
		 { ("building_basic_20", 0) , "B20_Traveler/prefabs/mc-traveler" },
		 { ("building_basic_21", 0) , "B21_Cat in bath/prefabs/mc-cat-in-bath" },
		 { ("building_basic_22", 0) , "B22_Strawberry Farmer/prefabs/mc-strawberry-farmer" },
		 { ("building_basic_23", 0) , "B23_Cacao Farmer/prefabs/mc-cacao-farmer" },
		 { ("building_basic_24", 0) , "B24_Seller/prefabs/mc-seller" },
		 { ("building_basic_25", 0) , "B25_Touris 1/prefabs/mc-tourist-1" },
		 { ("building_basic_26", 0) , "B26_BeachLifeguard/prefabs/mc-beach-lifeguard" },
		 { ("building_basic_27", 0) , "B27_Touris 2/prefabs/mc-tourist-2" },
		 //{ ("building_basic_28", 0) , "" },
		 { ("building_basic_29", 0) , "B29_Doctor/prefabs/mc-doctor" },
		 { ("building_basic_30", 0) , "B30_FireFighter/prefabs/mc-fire-fighter" },
		 { ("building_basic_31", 0) , "B31_Policecat/prefabs/mc-police-cat" },
	};

	//Load MC Prefab Editor

	public string GetPrefabPath_Editor()
	{
		if (AddressMapping.ContainsKey((buildingId, orderInBuilding)))
		{
			var prefabPath = AddressMapping[(buildingId, orderInBuilding)];
			if (string.IsNullOrEmpty(prefabPath))
			{
				Debug.LogWarning($"McOnMapPresenter > GetPrefabPath_Editor > AddressMapping for BuildingId {buildingId} OrderInBuilding {orderInBuilding} is invalid!");
				return null;
            }
			return prefabPath;
        }
		return null;
	}
}
#endif