# BiggerDrops

Bigger Drops is a mod for HBS BattleTech that adds on the ability to expand your drop size from 4 to 8 or more units

## Dependencies

- Mission Control: https://www.missioncontrolmod.com/

## Optional Dependencies

- Custom Units: https://github.com/CMiSSioN/CBDebugEnviroment/tree/master/CustomUnits


## Settings
Bigger drops is configurable through the following settings. The values shown here are the default values if not present in the settings.json file

*Note: Some settings are only applicable when Custom Units is present or not present in the load order*


```json
{
  "debugLog": false, 
  "additionalLanceName": "AI LANCE",
  "additinalMechSlots": 4,
  "additinalPlayerMechSlots": 4,
  "defaultMaxTonnage": 500,
  "allowUpgrades" : false,
  "showAdditionalArgoUpgrades" : false,
  "enable4thCategory": false,
  "argoUpgradeName" : "Command & Control",
  "argoUpgradeCategory1Name" : "Drop Size", 
  "argoUpgradeCategory2Name" : "Mech Control", 
  "argoUpgradeCategory3Name" : "Drop Tonnage",
  "argoUpgradeCategory4Name" : "Drop Information",
  "CuInitialVehicles" : 0,
  "respectFourDropLimit" : false,
  "limitFlashpointDrop" : true,
  "MAX_VEHICLE_SLOTS" : 8,
  "MAX_CU_DROP_SIZE" : 12,
  "MAX_CU_ADDITINAL_MECH_SLOTS":  8,
  "CuV2FormationSize": 4,
  "CuV2InitialSlots": []
}
```

### Always Applicable
the following settings are always applicable:

`debugLog`: Add additional logging, unless you are hunting a bug, set this to false

`defaultMaxTonnage` : The max tonnage you can drop by default. Note: when `allowUpgrades` is true, this is the max tonnage you can drop at the start of a career, 
upgrades/events can alter this value by changing the `BiggerDrops_MaxTonnage` stat. Also Note that contracts can override this value in all cases.

`allowUpgrades` : When true allows Argo Upgrades/events to increase or decrease the available drop tonnage and slots, when disabled the settings file setting will always apply.

`showAdditionalArgoUpgrades` : When true, an additional area on the Argo's upgrade screen becomes available for upgrades to appear in with 3 separate categories for upgrades.

`enable4thCategory` : Enables the use of the 4th category in the new upgrade bar.

`argoUpgradeName` :  When `showAdditionalArgoUpgrades` is true, this is the name of the new upgrades bar in the Argo's upgrade screen.

`argoUpgradeCategory1Name` : When `showAdditionalArgoUpgrades` is true, this is the name of the 1st category in the new upgrade bar

`argoUpgradeCategory2Name` : When `showAdditionalArgoUpgrades` is true, this is the name of the 2nd category in the new upgrade bar

`argoUpgradeCategory3Name` : When `showAdditionalArgoUpgrades` is true, this is the name of the 3rd category in the new upgrade bar

`argoUpgradeCategory4Name` : When `showAdditionalArgoUpgrades` is true, this is the name of the 4th category in the new upgrade bar

### When Running Without CustomUnits
the following settings are only applicable if CustomUnits mod is not present

`respectFourDropLimit`: When true, contracts max units must be set to -1 or else the unit count in the contract will be enforced (by default most contracts are set to 4),
contracts that limit you to less than 4 mechs will be restricted regardless of this setting

`limitFlashpointDrop` : when set to true, flashpoint drops will be limited to 4 mechs, setting this to false will allow additional slots to be available, 
provided MissionControl is also configured to allow for this

`additionalLanceName`: the name of the additional lance on the lance configuration screen

### When Running Without CustomUnits or With CustomUnits V1

the following settings are applicable when running without CustomUnits or when CustomUnits is present with V1 of its expanded lance API,
they are not applicable if the version of CustomUnits has V2 of the API available 

`additinalMechSlots`: the number of additional mech slots to add, valid range of:
- `0 - 4`, when Standalone
- up to the max defined by `MAX_CU_ADDITINAL_MECH_SLOTS` if CustomUnits is present.

if upgrades are enabled this is the starting number of slots for a career, events or upgrades can alter this value by changing the `BiggerDrops_AdditionalMechSlots` stat.
Note, if this value is greater than the value of `additinalPlayerMechSlots`, then extra units above that value will be assigned to the control of an allied AI player. 

Example: if `additinalMechSlots` is set to 3, but `additinalPlayerMechSlots` is set to 1, then the 2 extra units will be assigned to the allied AI.

*Note: Yes this setting is misspelled, however it has been left so for compatibility reasons*

`additinalPlayerMechSlots`: the number of additional mechs under the control of the player. valid range of:
- `0 - 4`, when Standalone
- up to the max defined by `MAX_CU_ADDITINAL_MECH_SLOTS` if CustomUnits is present.

this value cannot be greater then `additinalMechSlots`.

if upgrades are enabled, this value acts as the starting value for a career, events or upgrades can alter this value by changing the `BiggerDrops_AdditionalPlayerMechSlots` stat.

*Note: Yes this setting is misspelled, however it has been left so for compatibility reasons*

### When Running With CustomUnits V1

the following settings are applicable when CustomUnits is present with V1 of its expanded lance API,
they are not applicable if the version of CustomUnits has V2 of the API available 

Note: When CustomUnits V1 is present, all Mech Slots become capable of containing either a vehicle or a mech, provided vehicles are enabled by CU.

`CuInitialVehicles` the number of Vehicle only slots available. when upgrades are enabled, this is the starting value for a career, upgrades or events can alter this by changing the `BiggerDrops_CuVehicleCount` stat.
Valid range is `0` to the value of `MAX_VEHICLE_SLOTS`

`MAX_VEHICLE_SLOTS`: the maximum number of vehicle only slots that can be made available

`MAX_CU_DROP_SIZE`: the maximum number of drop slots available (mech + vehicle) that will be allowed.

`MAX_CU_ADDITINAL_MECH_SLOTS` : the maximum number of additional mech slots that can be made available

### When Running With CustomUnits V2

the following settings are only applicable when CustomUnits is present with V2 of its expanded lance API.

Note: In CU V2, lance's can be mixed slot types if desired.

`CuV2FormationSize` : the size of a lance when configuring your lance for drop and in mission. suggested values are `4 - 6` and should not exceed `8`.

`CuV2InitialSlots` : a list of `DropSlotDef` IDs that make up the initial configuration on a new career. if upgrades are disabled, then this will be your available drop slots.

DropSlotDefs (documented below) provide the actual descriptions of what each slot type is capable of containing and the order they will appear in on the lance configuration screen.

Example: 
```json
[
  "default_omni_slot",
  "default_omni_slot",
  "default_omni_slot",
  "default_omni_slot",
  "default_vehicle_slot",
  "default_vehicle_slot"
]
```
assuming the DropSlotDefs that define these slots do what their names imply, this would give the player 4 Omni (unit type) slots and 2 vehicle only slots at the start of their career.

## DropDescriptionDef
These objects are part of the `DropSlotDef` and used to define its ID

```json
{
    "Id": "default_omni_slot",
    "Name": "UNIVERSAL SLOT",
    "Details": "This type of drop slots can be used for all chassis",
    "Icon": ""
  }
```
`Id` : the ID for the DropSlotDef containing this descriptor

`Name` : Name of the slot

`Details` : any additional details about this slot

`Icon` : ??? used by CustomUnits

## DropSlotDef
These files are used to describe what a particular type of drop slot can hold and do. They are only applicable when CU V2 is present and must be loaded as a custom resource either by CU or by bigger drops itself.

```json
{
  "Description": {
    "Id": "default_omni_slot",
    "Name": "UNIVERSAL SLOT",
    "Details": "This type of drop slots can be used for all chassis",
    "Icon": ""
  },
  "Disabled": false,
  "PlayerControl": true,
  "Tags": [
    "can_drop_generic_mech",
    "can_drop_generic_vehicle",
    "can_drop_ba"
  ],
  "Decorations": [
    "decoration_slot_mech",
    "decoration_slot_vehicle",
    "decoration_slot_ba"
  ],
  "StatName": "BiggerDrops_AdditionalOmniSlots",
  "Order": 1,
  "SeparateLance": false,
  "HotDrop" : false,
  "combineLanceWith" : []
}
```

`Description` : a `DropDescriptionDef` that provides this Def with its ID

`Disabled` : whether or not this slot type is disabled by CU

`PlayerControl` : When true, units in this slot will be given to the player to control, when false they will be given to an allied AI

`Tags` : Drop Class Tags used to define what unit types can occupy this slot, See CustomUnits docs for details

`Decorations` : Drop Decoration Tags used to show players what type of slot this is, See CustomUnits docs for details

`StatName` : The Stat name for Bigger Drops to use to track how many of this slot type is available, upgrades/events can use this stat to alter available number of this slot type.
*Note: each DropSlotDef must be assigned a unique stat, stat name can be whatever you want*

`Order` : the order BiggerDrops will process this slot in comparision to other slot types. Defs with Order 1 will be shown first on the lance configuration screen, 
Defs with a higher order will shown later (in ascending order)

`SeparateLance` : when true this slot type will be separated into its own group of lances and not mixed with other slot types

`HotDrop` : This slot can be used for a delayed "Hot Drop" (See CustomUnits docs), when true, this slot type will automatically be treated as separate lance grouping

`combineLanceWith` : a list of other DropSlotDef Ids, when populated the slots mentioned here will be grouped into separate lances with this slot

## Lance Ordering

Lances will be displayed in the following order:

1. Mixed Lances (Lances consisting of DropSlotDefs where `SeparateLance` & `HotDrop` are false and `combineLanceWith` is empty)
2. Combined Lances (Lances of DropSlotDefs where `combineLanceWith` was not empty)
3. Separate Lances (Lances where `SeparateLance` was true & `HotDrop` is false)
4. Hot Drop Lances (Lances where, `HotDrop` was true)



