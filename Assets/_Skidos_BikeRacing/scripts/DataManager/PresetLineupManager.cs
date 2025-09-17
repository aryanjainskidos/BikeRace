namespace vasundharabikeracing {
using UnityEngine;
using System.Collections;
using Data_MainProject;


/**
 * tikai definé achívmentus
 * pagaidám HelperMenedźeris - galvená datu struktúra "Palettes" ir lietojama caur DataManager 
 */
public class PresetLineupManager : MonoBehaviour
{


    public static OrderedList_BikeRace<int, PresetRecord> DefinePresets()
    {
        OrderedList_BikeRace<int, PresetRecord> Presets = new OrderedList_BikeRace<int, PresetRecord>();

        //luudzu, lieto praatiigus nosaukumus - tie tiks suutiiti serverim, paarkraasoshanas gadiijumaa!

        Presets[0] = new PresetRecord("Rims_Black", new Color32[] { new Color32(0, 0, 0, 255) });
        Presets[1] = new PresetRecord("Rims_Yellow", new Color32[] { new Color32(255, 228, 0, 255) });
        Presets[2] = new PresetRecord("Rims_White", new Color32[] { new Color32(255, 255, 255, 255) });
        Presets[3] = new PresetRecord("Rims_Blue", new Color32[] { new Color32(49, 161, 246, 255) });
        Presets[4] = new PresetRecord("Rims_Red", new Color32[] { new Color32(255, 28, 17, 255) });
        Presets[5] = new PresetRecord("Rims_Green", new Color32[] { new Color32(108, 250, 4, 255) });
        Presets[6] = new PresetRecord("Rims_Silver", new Color32[] { new Color32(200, 200, 200, 255) });
        Presets[7] = new PresetRecord("Rims_Gold", new Color32[] { new Color32(232, 173, 20, 255) });
        Presets[8] = new PresetRecord("Rims_Purple", new Color32[] { new Color32(136, 40, 209, 255) });

        Presets[100] = new PresetRecord("Main_Helmet_Black", new Color32[] { new Color32(40, 40, 40, 255), new Color32(244, 22, 10, 255), new Color32(255, 255, 255, 255) });
        Presets[101] = new PresetRecord("Main_Helmet_Yellow", new Color32[] { new Color32(255, 200, 0, 255), new Color32(5, 5, 5, 255), new Color32(236, 236, 236, 255) });
        Presets[102] = new PresetRecord("Main_Helmet_White", new Color32[] { new Color32(240, 240, 240, 255), new Color32(5, 5, 5, 255), new Color32(236, 236, 236, 255) });
        Presets[103] = new PresetRecord("Main_Helmet_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(254, 254, 254, 255), new Color32(236, 236, 236, 255) });
        Presets[104] = new PresetRecord("Main_Helmet_Red", new Color32[] { new Color32(253, 29, 17, 255), new Color32(254, 253, 249, 255), new Color32(236, 236, 236, 255) });
        Presets[105] = new PresetRecord("Main_Helmet_Green", new Color32[] { new Color32(174, 237, 41, 255), new Color32(255, 255, 255, 255), new Color32(174, 237, 41, 255) });

        Presets[110] = new PresetRecord("Main_Body_Black", new Color32[] { new Color32(34, 34, 34, 255), new Color32(241, 17, 5, 255), new Color32(255, 255, 255, 255), new Color32(255, 223, 13, 255) });
        Presets[111] = new PresetRecord("Main_Body_Yellow", new Color32[] { new Color32(255, 200, 0, 255), new Color32(15, 15, 15, 255), new Color32(255, 255, 255, 255), new Color32(255, 200, 0, 255) });
        Presets[112] = new PresetRecord("Main_Body_White", new Color32[] { new Color32(240, 240, 240, 255), new Color32(8, 8, 8, 255), new Color32(36, 36, 36, 255), new Color32(230, 230, 230, 255) });
        Presets[113] = new PresetRecord("Main_Body_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(238, 238, 238, 255), new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 255) });
        Presets[114] = new PresetRecord("Main_Body_Red", new Color32[] { new Color32(248, 39, 27, 255), new Color32(238, 238, 238, 255), new Color32(255, 255, 255, 255), new Color32(255, 223, 13, 255) });
        Presets[115] = new PresetRecord("Main_Body_Green", new Color32[] { new Color32(174, 237, 41, 255), new Color32(106, 22, 214, 255), new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255) });

        Presets[120] = new PresetRecord("Main_Bike_Black", new Color32[] { new Color32(35, 35, 35, 255), new Color32(85, 85, 85, 255) });
        Presets[121] = new PresetRecord("Main_Bike_Yellow", new Color32[] { new Color32(252, 214, 2, 255), new Color32(232, 121, 3, 255) });
        Presets[122] = new PresetRecord("Main_Bike_White", new Color32[] { new Color32(250, 250, 250, 255), new Color32(165, 165, 165, 255) });
        Presets[123] = new PresetRecord("Main_Bike_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(245, 245, 245, 255) });
        Presets[124] = new PresetRecord("Main_Bike_Red", new Color32[] { new Color32(238, 21, 12, 255), new Color32(251, 189, 1, 255) });
        Presets[125] = new PresetRecord("Main_Bike_Green", new Color32[] { new Color32(159, 228, 62, 255), new Color32(255, 255, 255, 255) });

        Presets[200] = new PresetRecord("Beach_Helmet_Black", new Color32[] { new Color32(15, 15, 15, 255), new Color32(242, 13, 13, 255) });
        Presets[201] = new PresetRecord("Beach_Helmet_Yellow", new Color32[] { new Color32(255, 206, 11, 255), new Color32(8, 8, 8, 255) });
        Presets[202] = new PresetRecord("Beach_Helmet_White", new Color32[] { new Color32(255, 255, 255, 255), new Color32(17, 17, 17, 255) });
        Presets[203] = new PresetRecord("Beach_Helmet_Blue", new Color32[] { new Color32(0, 121, 215, 255), new Color32(255, 255, 255, 255) });
        Presets[204] = new PresetRecord("Beach_Helmet_Red", new Color32[] { new Color32(254, 47, 13, 255), new Color32(255, 255, 255, 255) });
        Presets[205] = new PresetRecord("Beach_Helmet_Green", new Color32[] { new Color32(176, 252, 16, 255), new Color32(109, 15, 180, 255) });

        Presets[210] = new PresetRecord("Beach_Body_Black", new Color32[] { new Color32(8, 8, 8, 255), new Color32(196, 40, 0, 255) });
        Presets[211] = new PresetRecord("Beach_Body_Yellow", new Color32[] { new Color32(255, 209, 0, 255), new Color32(50, 45, 44, 255) });
        Presets[212] = new PresetRecord("Beach_Body_White", new Color32[] { new Color32(255, 255, 255, 255), new Color32(255, 253, 253, 255) });
        Presets[213] = new PresetRecord("Beach_Body_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(255, 253, 253, 255) });
        Presets[214] = new PresetRecord("Beach_Body_Red", new Color32[] { new Color32(249, 29, 17, 255), new Color32(255, 253, 253, 255) });
        Presets[215] = new PresetRecord("Beach_Body_Green", new Color32[] { new Color32(176, 252, 16, 255), new Color32(121, 64, 224, 255) });

        Presets[220] = new PresetRecord("Beach_Bike_Black", new Color32[] { new Color32(31, 31, 31, 255), new Color32(242, 13, 13, 255) });
        Presets[221] = new PresetRecord("Beach_Bike_Yellow", new Color32[] { new Color32(255, 223, 11, 255), new Color32(8, 8, 8, 255) });
        Presets[222] = new PresetRecord("Beach_Bike_White", new Color32[] { new Color32(244, 244, 244, 255), new Color32(17, 17, 17, 255) });
        Presets[223] = new PresetRecord("Beach_Bike_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(255, 255, 255, 255) });
        Presets[224] = new PresetRecord("Beach_Bike_Red", new Color32[] { new Color32(251, 49, 27, 255), new Color32(255, 255, 255, 255) });
        Presets[225] = new PresetRecord("Beach_Bike_Green", new Color32[] { new Color32(176, 252, 16, 255), new Color32(109, 15, 180, 255) });

        Presets[300] = new PresetRecord("Agent_Helmet_Black", new Color32[] { new Color32(45, 45, 45, 255), new Color32(0, 0, 0, 255), new Color32(210, 210, 210, 255) });
        Presets[301] = new PresetRecord("Agent_Helmet_Yellow", new Color32[] { new Color32(255, 206, 11, 255), new Color32(254, 252, 185, 255), new Color32(244, 244, 244, 255) });
        Presets[302] = new PresetRecord("Agent_Helmet_White", new Color32[] { new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255), new Color32(244, 244, 244, 255) });
        Presets[303] = new PresetRecord("Agent_Helmet_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(255, 255, 255, 255), new Color32(244, 244, 244, 255) });
        Presets[304] = new PresetRecord("Agent_Helmet_Red", new Color32[] { new Color32(254, 47, 13, 255), new Color32(176, 15, 10, 255), new Color32(244, 244, 244, 255) });
        Presets[305] = new PresetRecord("Agent_Helmet_Green", new Color32[] { new Color32(176, 252, 16, 255), new Color32(125, 219, 3, 255), new Color32(244, 244, 244, 255) });

        Presets[310] = new PresetRecord("Agent_Body_Black", new Color32[] { new Color32(10, 10, 10, 255), new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 255), new Color32(255, 255, 255, 255), new Color32(26, 26, 26, 255) });
        Presets[311] = new PresetRecord("Agent_Body_Yellow", new Color32[] { new Color32(255, 209, 0, 255), new Color32(10, 10, 10, 255), new Color32(0, 0, 0, 255), new Color32(10, 10, 10, 255), new Color32(255, 255, 255, 255) });
        Presets[312] = new PresetRecord("Agent_Body_White", new Color32[] { new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 255), new Color32(10, 10, 10, 255), new Color32(26, 26, 26, 255) });
        Presets[313] = new PresetRecord("Agent_Body_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255), new Color32(4, 129, 226, 255) });
        Presets[314] = new PresetRecord("Agent_Body_Red", new Color32[] { new Color32(251, 49, 27, 255), new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 255), new Color32(255, 255, 255, 255), new Color32(26, 26, 26, 255) });
        Presets[315] = new PresetRecord("Agent_Body_Green", new Color32[] { new Color32(176, 252, 16, 255), new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255), new Color32(127, 67, 237, 255), new Color32(255, 255, 255, 255) });

        Presets[320] = new PresetRecord("Agent_Bike_Black", new Color32[] { new Color32(27, 27, 27, 255), new Color32(214, 159, 16, 255) });
        Presets[321] = new PresetRecord("Agent_Bike_Yellow", new Color32[] { new Color32(255, 206, 11, 255), new Color32(10, 10, 10, 255) });
        Presets[322] = new PresetRecord("Agent_Bike_White", new Color32[] { new Color32(255, 255, 255, 255), new Color32(37, 37, 37, 255) });
        Presets[323] = new PresetRecord("Agent_Bike_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(250, 250, 250, 255) });
        Presets[324] = new PresetRecord("Agent_Bike_Red", new Color32[] { new Color32(233, 31, 13, 255), new Color32(250, 250, 250, 255) });
        Presets[325] = new PresetRecord("Agent_Bike_Green", new Color32[] { new Color32(176, 252, 16, 255), new Color32(137, 66, 207, 255) });

        Presets[400] = new PresetRecord("Tourist_Helmet_Black", new Color32[] { new Color32(38, 38, 38, 255), new Color32(177, 177, 177, 255), new Color32(210, 210, 210, 255) });
        Presets[401] = new PresetRecord("Tourist_Helmet_Yellow", new Color32[] { new Color32(238, 158, 3, 255), new Color32(255, 222, 0, 255), new Color32(253, 246, 210, 255) });
        Presets[402] = new PresetRecord("Tourist_Helmet_White", new Color32[] { new Color32(244, 244, 244, 255), new Color32(135, 135, 135, 255), new Color32(250, 250, 250, 255) });
        Presets[403] = new PresetRecord("Tourist_Helmet_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(236, 247, 255, 255), new Color32(236, 236, 236, 255) });
        Presets[404] = new PresetRecord("Tourist_Helmet_Red", new Color32[] { new Color32(193, 33, 0, 255), new Color32(254, 247, 243, 255), new Color32(254, 247, 243, 255) });
        Presets[405] = new PresetRecord("Tourist_Helmet_Green", new Color32[] { new Color32(148, 245, 23, 255), new Color32(126, 57, 196, 255), new Color32(250, 250, 250, 255) });

        Presets[410] = new PresetRecord("Tourist_Body_Black", new Color32[] { new Color32(85, 85, 85, 255), new Color32(255, 40, 11, 255) });
        Presets[411] = new PresetRecord("Tourist_Body_Yellow", new Color32[] { new Color32(255, 210, 0, 255), new Color32(50, 50, 50, 255) });
        Presets[412] = new PresetRecord("Tourist_Body_White", new Color32[] { new Color32(244, 244, 244, 255), new Color32(56, 56, 56, 255) });
        Presets[413] = new PresetRecord("Tourist_Body_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(58, 78, 227, 255) });
        Presets[414] = new PresetRecord("Tourist_Body_Red", new Color32[] { new Color32(219, 32, 33, 255), new Color32(51, 15, 5, 255) });
        Presets[415] = new PresetRecord("Tourist_Body_Green", new Color32[] { new Color32(148, 245, 23, 255), new Color32(112, 64, 200, 255) });

        Presets[420] = new PresetRecord("Tourist_Bike_Black", new Color32[] { new Color32(33, 33, 33, 255), new Color32(255, 40, 11, 255) });
        Presets[421] = new PresetRecord("Tourist_Bike_Yellow", new Color32[] { new Color32(255, 210, 0, 255), new Color32(5, 5, 5, 255) });
        Presets[422] = new PresetRecord("Tourist_Bike_White", new Color32[] { new Color32(237, 237, 237, 255), new Color32(31, 31, 31, 255) });
        Presets[423] = new PresetRecord("Tourist_Bike_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(255, 255, 255, 255) });
        Presets[424] = new PresetRecord("Tourist_Bike_Red", new Color32[] { new Color32(219, 33, 20, 255), new Color32(255, 255, 255, 255) });
        Presets[425] = new PresetRecord("Tourist_Bike_Green", new Color32[] { new Color32(148, 245, 23, 255), new Color32(255, 255, 255, 255) });

        Presets[500] = new PresetRecord("Hipster_Helmet_Black", new Color32[] { new Color32(60, 60, 60, 255), new Color32(240, 240, 240, 255) });
        Presets[501] = new PresetRecord("Hipster_Helmet_Yellow", new Color32[] { new Color32(255, 234, 0, 255), new Color32(254, 252, 241, 255) });
        Presets[502] = new PresetRecord("Hipster_Helmet_White", new Color32[] { new Color32(233, 233, 233, 255), new Color32(250, 250, 250, 255) });
        Presets[503] = new PresetRecord("Hipster_Helmet_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(236, 236, 236, 255) });
        Presets[504] = new PresetRecord("Hipster_Helmet_Red", new Color32[] { new Color32(255, 42, 29, 255), new Color32(245, 245, 245, 255) });
        Presets[505] = new PresetRecord("Hipster_Helmet_Green", new Color32[] { new Color32(187, 255, 4, 255), new Color32(245, 245, 245, 255) });

        Presets[510] = new PresetRecord("Hipster_Body_Black", new Color32[] { new Color32(102, 102, 102, 255), new Color32(48, 48, 48, 255) });
        Presets[511] = new PresetRecord("Hipster_Body_Yellow", new Color32[] { new Color32(255, 234, 0, 255), new Color32(253, 165, 46, 255) });
        Presets[512] = new PresetRecord("Hipster_Body_White", new Color32[] { new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255) });
        Presets[513] = new PresetRecord("Hipster_Body_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(255, 255, 255, 255) });
        Presets[514] = new PresetRecord("Hipster_Body_Red", new Color32[] { new Color32(255, 27, 32, 255), new Color32(255, 255, 255, 255) });
        Presets[515] = new PresetRecord("Hipster_Body_Green", new Color32[] { new Color32(187, 255, 4, 255), new Color32(133, 91, 231, 255) });

        Presets[520] = new PresetRecord("Hipster_Bike_Black", new Color32[] { new Color32(10, 10, 10, 255), new Color32(233, 233, 233, 255) });
        Presets[521] = new PresetRecord("Hipster_Bike_Yellow", new Color32[] { new Color32(255, 216, 0, 255), new Color32(35, 35, 35, 255) });
        Presets[522] = new PresetRecord("Hipster_Bike_White", new Color32[] { new Color32(240, 240, 240, 255), new Color32(56, 56, 56, 255) });
        Presets[523] = new PresetRecord("Hipster_Bike_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(255, 255, 255, 255) });
        Presets[524] = new PresetRecord("Hipster_Bike_Red", new Color32[] { new Color32(221, 35, 26, 255), new Color32(255, 255, 255, 255) });
        Presets[525] = new PresetRecord("Hipster_Bike_Green", new Color32[] { new Color32(170, 242, 0, 255), new Color32(111, 66, 216, 255) });

        Presets[600] = new PresetRecord("Racer_Helmet_Black", new Color32[] { new Color32(37, 37, 37, 255), new Color32(108, 108, 108, 255), new Color32(150, 150, 150, 255) });
        Presets[601] = new PresetRecord("Racer_Helmet_Yellow", new Color32[] { new Color32(255, 234, 0, 255), new Color32(151, 151, 150, 255), new Color32(168, 168, 168, 255) });
        Presets[602] = new PresetRecord("Racer_Helmet_White", new Color32[] { new Color32(255, 255, 255, 255), new Color32(50, 50, 50, 255), new Color32(50, 50, 50, 255) });
        Presets[603] = new PresetRecord("Racer_Helmet_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(183, 193, 252, 255), new Color32(134, 219, 254, 255) });
        Presets[604] = new PresetRecord("Racer_Helmet_Red", new Color32[] { new Color32(255, 40, 11, 255), new Color32(177, 0, 8, 255), new Color32(177, 0, 8, 255) });
        Presets[605] = new PresetRecord("Racer_Helmet_Green", new Color32[] { new Color32(149, 241, 0, 255), new Color32(2, 205, 21, 255), new Color32(2, 205, 21, 255) });

        Presets[610] = new PresetRecord("Racer_Body_Black", new Color32[] { new Color32(40, 40, 40, 255), new Color32(255, 30, 7, 255), new Color32(245, 245, 245, 255) });
        Presets[611] = new PresetRecord("Racer_Body_Yellow", new Color32[] { new Color32(255, 234, 0, 255), new Color32(36, 36, 36, 255), new Color32(168, 168, 168, 255) });
        Presets[612] = new PresetRecord("Racer_Body_White", new Color32[] { new Color32(255, 255, 255, 255), new Color32(40, 40, 40, 255), new Color32(50, 50, 50, 255) });
        Presets[613] = new PresetRecord("Racer_Body_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(255, 255, 255, 255), new Color32(134, 219, 254, 255) });
        Presets[614] = new PresetRecord("Racer_Body_Red", new Color32[] { new Color32(255, 40, 11, 255), new Color32(255, 255, 255, 255), new Color32(163, 4, 1, 255) });
        Presets[615] = new PresetRecord("Racer_Body_Green", new Color32[] { new Color32(173, 244, 0, 255), new Color32(109, 52, 174, 255), new Color32(2, 205, 21, 255) });

        Presets[620] = new PresetRecord("Racer_Bike_Black", new Color32[] { new Color32(32, 32, 32, 255), new Color32(255, 30, 7, 255) });
        Presets[621] = new PresetRecord("Racer_Bike_Yellow", new Color32[] { new Color32(254, 233, 3, 255), new Color32(25, 25, 24, 255) });
        Presets[622] = new PresetRecord("Racer_Bike_White", new Color32[] { new Color32(246, 246, 246, 255), new Color32(33, 33, 33, 255) });
        Presets[623] = new PresetRecord("Racer_Bike_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(254, 254, 254, 255) });
        Presets[624] = new PresetRecord("Racer_Bike_Red", new Color32[] { new Color32(230, 29, 24, 255), new Color32(254, 254, 254, 255) });
        Presets[625] = new PresetRecord("Racer_Bike_Green", new Color32[] { new Color32(163, 244, 0, 255), new Color32(97, 15, 145, 255) });

        Presets[700] = new PresetRecord("Astro_Helmet_Black", new Color32[] { new Color32(37, 37, 37, 255), new Color32(231, 57, 40, 255) });
        Presets[701] = new PresetRecord("Astro_Helmet_Yellow", new Color32[] { new Color32(255, 200, 0, 255), new Color32(5, 5, 5, 255) });
        Presets[702] = new PresetRecord("Astro_Helmet_White", new Color32[] { new Color32(255, 255, 255, 255), new Color32(200, 200, 200, 255) });
        Presets[703] = new PresetRecord("Astro_Helmet_Blue", new Color32[] { new Color32(37, 151, 239, 255), new Color32(236, 236, 236, 255) });
        Presets[704] = new PresetRecord("Astro_Helmet_Red", new Color32[] { new Color32(252, 40, 30, 255), new Color32(255, 255, 255, 255) });
        Presets[705] = new PresetRecord("Astro_Helmet_Green", new Color32[] { new Color32(166, 255, 10, 255), new Color32(133, 68, 216, 255) });

        Presets[710] = new PresetRecord("Astro_Body_Black", new Color32[] { new Color32(40, 40, 40, 255), new Color32(229, 39, 26, 255) });
        Presets[711] = new PresetRecord("Astro_Body_Yellow", new Color32[] { new Color32(255, 200, 0, 255), new Color32(45, 43, 43, 255), });
        Presets[712] = new PresetRecord("Astro_Body_White", new Color32[] { new Color32(255, 255, 255, 255), new Color32(200, 200, 200, 255) });
        Presets[713] = new PresetRecord("Astro_Body_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(241, 241, 241, 255) });
        Presets[714] = new PresetRecord("Astro_Body_Red", new Color32[] { new Color32(252, 40, 30, 255), new Color32(245, 245, 245, 255) });
        Presets[715] = new PresetRecord("Astro_Body_Green", new Color32[] { new Color32(166, 255, 10, 255), new Color32(114, 50, 196, 255) });

        Presets[720] = new PresetRecord("Astro_Bike_Black", new Color32[] { new Color32(35, 35, 35, 255), new Color32(165, 18, 3, 255) });
        Presets[721] = new PresetRecord("Astro_Bike_Yellow", new Color32[] { new Color32(255, 200, 0, 255), new Color32(250, 246, 218, 255) });
        Presets[722] = new PresetRecord("Astro_Bike_White", new Color32[] { new Color32(246, 246, 246, 255), new Color32(170, 170, 170, 255) });
        Presets[723] = new PresetRecord("Astro_Bike_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(187, 227, 245, 255) });
        Presets[724] = new PresetRecord("Astro_Bike_Red", new Color32[] { new Color32(222, 35, 27, 255), new Color32(255, 223, 220, 255) });
        Presets[725] = new PresetRecord("Astro_Bike_Green", new Color32[] { new Color32(166, 255, 10, 255), new Color32(250, 250, 250, 255) });

        Presets[800] = new PresetRecord("Robot_Helmet_Black", new Color32[] { new Color32(15, 15, 15, 255), new Color32(241, 22, 22, 255) });
        Presets[801] = new PresetRecord("Robot_Helmet_Yellow", new Color32[] { new Color32(255, 200, 0, 255), new Color32(0, 0, 0, 255) });
        Presets[802] = new PresetRecord("Robot_Helmet_White", new Color32[] { new Color32(255, 255, 255, 255), new Color32(5, 5, 5, 255) });
        Presets[803] = new PresetRecord("Robot_Helmet_Blue", new Color32[] { new Color32(3, 111, 195, 255), new Color32(254, 254, 254, 255) });
        Presets[804] = new PresetRecord("Robot_Helmet_Red", new Color32[] { new Color32(250, 42, 21, 255), new Color32(254, 253, 249, 255) });
        Presets[805] = new PresetRecord("Robot_Helmet_Green", new Color32[] { new Color32(176, 252, 17, 255), new Color32(114, 50, 196, 255) });

        Presets[810] = new PresetRecord("Robot_Body_Black", new Color32[] { new Color32(34, 34, 34, 255), new Color32(228, 44, 34, 255) });
        Presets[811] = new PresetRecord("Robot_Body_Yellow", new Color32[] { new Color32(255, 200, 0, 255), new Color32(5, 5, 5, 255) });
        Presets[812] = new PresetRecord("Robot_Body_White", new Color32[] { new Color32(240, 240, 240, 255), new Color32(45, 43, 43, 255) });
        Presets[813] = new PresetRecord("Robot_Body_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(238, 238, 238, 255) });
        Presets[814] = new PresetRecord("Robot_Body_Red", new Color32[] { new Color32(250, 42, 21, 255), new Color32(238, 238, 238, 255) });
        Presets[815] = new PresetRecord("Robot_Body_Green", new Color32[] { new Color32(176, 252, 17, 255), new Color32(114, 50, 196, 255) });

        Presets[820] = new PresetRecord("Robot_Bike_Black", new Color32[] { new Color32(31, 31, 31, 255), new Color32(228, 32, 32, 255) });
        Presets[821] = new PresetRecord("Robot_Bike_Yellow", new Color32[] { new Color32(240, 200, 0, 255), new Color32(0, 0, 0, 255) });
        Presets[822] = new PresetRecord("Robot_Bike_White", new Color32[] { new Color32(250, 250, 250, 255), new Color32(5, 5, 5, 255) });
        Presets[823] = new PresetRecord("Robot_Bike_Blue", new Color32[] { new Color32(4, 129, 226, 225), new Color32(254, 254, 254, 255) });
        Presets[824] = new PresetRecord("Robot_Bike_Red", new Color32[] { new Color32(222, 35, 27, 255), new Color32(254, 253, 249, 255) });
        Presets[825] = new PresetRecord("Robot_Bike_Green", new Color32[] { new Color32(176, 252, 17, 255), new Color32(114, 50, 196, 255) });

        Presets[900] = new PresetRecord("Punk_Helmet_Black", new Color32[] { new Color32(45, 45, 45, 255), new Color32(45, 45, 45, 255) });
        Presets[901] = new PresetRecord("Punk_Helmet_Yellow", new Color32[] { new Color32(255, 210, 0, 255), new Color32(50, 50, 50, 255) });
        Presets[902] = new PresetRecord("Punk_Helmet_White", new Color32[] { new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255) });
        Presets[903] = new PresetRecord("Punk_Helmet_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(255, 255, 255, 255) });
        Presets[904] = new PresetRecord("Punk_Helmet_Red", new Color32[] { new Color32(255, 40, 24, 255), new Color32(50, 50, 50, 255) });
        Presets[905] = new PresetRecord("Punk_Helmet_Green", new Color32[] { new Color32(160, 243, 25, 255), new Color32(114, 50, 196, 255) });

        Presets[910] = new PresetRecord("Punk_Body_Black", new Color32[] { new Color32(48, 48, 48, 255), new Color32(52, 52, 52, 255) });
        Presets[911] = new PresetRecord("Punk_Body_Yellow", new Color32[] { new Color32(255, 210, 0, 255), new Color32(45, 43, 43, 255) });
        Presets[912] = new PresetRecord("Punk_Body_White", new Color32[] { new Color32(255, 255, 255, 255), new Color32(45, 43, 43, 255) });
        Presets[913] = new PresetRecord("Punk_Body_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(238, 238, 238, 255) });
        Presets[914] = new PresetRecord("Punk_Body_Red", new Color32[] { new Color32(237, 38, 27, 255), new Color32(238, 238, 238, 255) });
        Presets[915] = new PresetRecord("Punk_Body_Green", new Color32[] { new Color32(174, 237, 41, 255), new Color32(114, 50, 196, 255) });

        Presets[920] = new PresetRecord("Punk_Bike_Black", new Color32[] { new Color32(31, 31, 31, 255), new Color32(240, 240, 240, 255) });
        Presets[921] = new PresetRecord("Punk_Bike_Yellow", new Color32[] { new Color32(240, 200, 0, 255), new Color32(45, 45, 45, 255) });
        Presets[922] = new PresetRecord("Punk_Bike_White", new Color32[] { new Color32(250, 250, 250, 255), new Color32(30, 30, 30, 255) });
        Presets[923] = new PresetRecord("Punk_Bike_Blue", new Color32[] { new Color32(4, 129, 226, 255), new Color32(240, 240, 240, 255) });
        Presets[924] = new PresetRecord("Punk_Bike_Red", new Color32[] { new Color32(222, 35, 27, 255), new Color32(240, 240, 240, 255) });
        Presets[925] = new PresetRecord("Punk_Bike_Green", new Color32[] { new Color32(159, 228, 62, 255), new Color32(240, 240, 240, 255) });

        Presets[1000] = new PresetRecord("Gold_Helmet_Gold", new Color32[] { new Color32(214, 159, 16, 255), new Color32(214, 159, 16, 255) });

        Presets[1010] = new PresetRecord("Gold_Body_Gold", new Color32[] { new Color32(214, 159, 16, 255), new Color32(214, 159, 16, 255) });

        Presets[1020] = new PresetRecord("Gold_Bike_Gold", new Color32[] { new Color32(214, 159, 16, 255), new Color32(214, 159, 16, 255) });


        return Presets;
    }

}

/**
 * JSONá saglabájama datu struktúra
 */
public class PresetRecord
{

    public PresetRecord(string name, Color32[] colors)
    {
        Name = name;
        Colors = colors;
    }

    public string Name; //for display purposes
    public Color32[] Colors;
}


}
