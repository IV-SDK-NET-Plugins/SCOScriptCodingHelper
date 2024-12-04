#include <natives.h>
#include <common.h>
#include <strings.h>
#include <types.h>
#include <consts.h>

void main(void)
{
	float offsetX = 0.0F;
	float offsetY = 0.0F;
	float offsetZ = 0.0F;
	int r = 255;
	int g = 0;
	int b = 0;
	int a = 100;
	bool drawCircleAroundPlayer = true;

	// Calling this allows the script to actually create widget groups and all the other debugging related stuff
	DEBUG_ON();

	// Widget Group Tests
	CREATE_WIDGET_GROUP("GROUPED WIDGET WINDOW"); // Creates a new widget group (A new window)

	CREATE_WIDGET_GROUP("Checkpoint"); // We already called CREATE_WIDGET_GROUP before, 
									   // so this time this creates a tab called "Checkpoint" within the "GROUPED WIDGET WINDOW" widget group

	ADD_WIDGET_TOGGLE("drawCircleAroundPlayer", &drawCircleAroundPlayer);

	ADD_WIDGET_FLOAT_SLIDER("X", &offsetX, -1000.0F, 1000.0F, 1.0F);
	ADD_WIDGET_FLOAT_SLIDER("Y", &offsetY, -1000.0F, 1000.0F, 1.0F);
	ADD_WIDGET_FLOAT_SLIDER("Z", &offsetZ, -1000.0F, 1000.0F, 1.0F);

	ADD_WIDGET_SLIDER("R", &r, 0, 255, 1.0F);
	ADD_WIDGET_SLIDER("G", &g, 0, 255, 1.0F);
	ADD_WIDGET_SLIDER("B", &b, 0, 255, 1.0F);
	ADD_WIDGET_SLIDER("A", &a, 0, 255, 1.0F);

	END_WIDGET_GROUP(); // Ends the "Checkpoint" widget group
	
	CREATE_WIDGET_GROUP("Another tab item"); // Creates another tab called "Another tab item" within the "GROUPED WIDGET WINDOW" widget group
	ADD_WIDGET_STRING("With just some text in it");
	END_WIDGET_GROUP(); // Ends the "Another tab item" widget group

	CREATE_WIDGET_GROUP("Yet another tab item"); // Creates another tab called "Yet another tab item" within the "GROUPED WIDGET WINDOW" widget group

	CREATE_WIDGET_GROUP("Test1"); // AND This now creates a tab within the "Yet another tab item" tab
	ADD_WIDGET_STRING("Text within the Test1 tab");
	END_WIDGET_GROUP(); // Ends the "Test1" widget group

	CREATE_WIDGET_GROUP("Test2"); // And this also creates a tab within the "Yet another tab item" tab
	ADD_WIDGET_STRING("Text within the Test2 tab");

	END_WIDGET_GROUP(); // Ends the "Test2" widget group

	END_WIDGET_GROUP(); // Ends the "Yet another tab item" widget group

	END_WIDGET_GROUP(); // Now we end the "GROUPED WIDGET WINDOW" widget group.
						// After this, when we call CREATE_WIDGET_GROUP again this will then create a whole new widget group (window)


	// Let's create another widget group but this time it wont be a grouped one
	CREATE_WIDGET_GROUP("NON-GROUPED WIDGET GROUP");

	ADD_WIDGET_STRING("Hello World!");

	END_WIDGET_GROUP(); // Ends the "NON-GROUPED WIDGET GROUP" widget group


	// Debug file test
	//OPEN_DEBUG_FILE();

	//SAVE_FLOAT_TO_DEBUG_FILE(offsetX);
	//SAVE_INT_TO_DEBUG_FILE(r);
	//SAVE_NEWLINE_TO_DEBUG_FILE();
	//SAVE_STRING_TO_DEBUG_FILE("TEST STRING!");
	//SAVE_STRING_TO_DEBUG_FILE(" another test to see the text be written on the same line");

	//CLOSE_DEBUG_FILE();

	while (TRUE)
	{

		// Get latest IV-SDK .NET console command
		char* consoleCmd[32];
		GET_LATEST_CONSOLE_COMMAND(&consoleCmd);

		if (COMPARE_STRING(consoleCmd, "kill_player"))
		{
			TASK_DIE(GetPlayerPed());
			RESET_LATEST_CONSOLE_COMMAND();
		}

		if (drawCircleAroundPlayer)
		{
			float pX, pY, pZ;
			GET_CHAR_COORDINATES(GetPlayerPed(), &pX, &pY, &pZ);
			DRAW_CHECKPOINT_WITH_ALPHA(pX + offsetX, pY + offsetY, pZ + offsetZ, 1.0F, r, g, b, a);
		}

		WAIT(0);
	}
}
