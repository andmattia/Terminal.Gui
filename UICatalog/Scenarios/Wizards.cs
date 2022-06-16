﻿using NStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terminal.Gui;

namespace UICatalog.Scenarios {
	[ScenarioMetadata (Name: "Wizards", Description: "Demonstrates how to the Wizard class")]
	[ScenarioCategory ("Dialogs")]
	public class Wizards : Scenario {
		public override void Setup ()
		{
			Win.ColorScheme = Colors.Base;
			var frame = new FrameView ("Wizard Options") {
				X = Pos.Center (),
				Y = 0,
				Width = Dim.Percent (75),
				ColorScheme = Colors.Base,
			};
			Win.Add (frame);

			var label = new Label ("Width:") {
				X = 0,
				Y = 0,
				Width = 15,
				Height = 1,
				TextAlignment = Terminal.Gui.TextAlignment.Right,
			};
			frame.Add (label);
			var widthEdit = new TextField ("80") {
				X = Pos.Right (label) + 1,
				Y = Pos.Top (label),
				Width = 5,
				Height = 1
			};
			frame.Add (widthEdit);

			label = new Label ("Height:") {
				X = 0,
				Y = Pos.Bottom (label),
				Width = Dim.Width (label),
				Height = 1,
				TextAlignment = Terminal.Gui.TextAlignment.Right,
			};
			frame.Add (label);
			var heightEdit = new TextField ("20") {
				X = Pos.Right (label) + 1,
				Y = Pos.Top (label),
				Width = 5,
				Height = 1
			};
			frame.Add (heightEdit);

			label = new Label ("Title:") {
				X = 0,
				Y = Pos.Bottom (label),
				Width = Dim.Width (label),
				Height = 1,
				TextAlignment = Terminal.Gui.TextAlignment.Right,
			};
			frame.Add (label);
			var titleEdit = new TextField ("Title") {
				X = Pos.Right (label) + 1,
				Y = Pos.Top (label),
				Width = Dim.Fill (),
				Height = 1
			};
			frame.Add (titleEdit);

			var useStepView = new CheckBox () {
				Text = "Add 3rd step controls to WizardStep instead of WizardStep.Controls",
				Checked = false,
				X = Pos.Left (titleEdit),
				Y = Pos.Bottom (titleEdit)
			};
			frame.Add (useStepView);


			void Top_Loaded ()
			{
				frame.Height = Dim.Height (widthEdit) + Dim.Height (heightEdit) + Dim.Height (titleEdit) + Dim.Height (useStepView) + 2;
				Top.Loaded -= Top_Loaded;
			}
			Top.Loaded += Top_Loaded;

			label = new Label ("Action:") {
				X = Pos.Center (),
				Y = Pos.AnchorEnd (1),
				AutoSize = true,
				TextAlignment = Terminal.Gui.TextAlignment.Right,
			};
			Win.Add (label);
			var actionLabel = new Label (" ") {
				X = Pos.Right (label),
				Y = Pos.AnchorEnd (1),
				AutoSize = true,
				ColorScheme = Colors.Error,
			};

			var showWizardButton = new Button ("Show Wizard") {
				X = Pos.Center (),
				Y = Pos.Bottom (frame) + 2,
				IsDefault = true,
			};
			showWizardButton.Clicked += () => {
				try {
					int width = 0;
					int.TryParse (widthEdit.Text.ToString (), out width);
					int height = 0;
					int.TryParse (heightEdit.Text.ToString (), out height);

					if (width < 1 || height < 1) {
						MessageBox.ErrorQuery ("Nope", "Height and width must be greater than 0 (much bigger)", "Ok");
						return;
					}

					var wizard = new Wizard (titleEdit.Text) {
						Width = width,
						Height = height
					};

					wizard.MovingBack += (args) => {
						//args.Cancel = true;
						actionLabel.Text = "Moving Back";
					};

					wizard.MovingNext += (args) => {
						//args.Cancel = true;
						actionLabel.Text = "Moving Next";
					};

					wizard.Finished += (args) => {
						//args.Cancel = true;
						actionLabel.Text = "Finished";
					};

					wizard.Cancelled += (args) => {
						//args.Cancel = true;
						actionLabel.Text = "Cancelled";
					};

					// Add 1st step
					var firstStep = new Wizard.WizardStep ("End User License Agreement");
					wizard.AddStep (firstStep);
					firstStep.ShowControls = false;
					firstStep.NextButtonText = "Accept!";
					firstStep.HelpText = "This is the End User License Agreement.\n\n\n\n\n\nThis is a test of the emergency broadcast system. This is a test of the emergency broadcast system.\nThis is a test of the emergency broadcast system.\n\n\nThis is a test of the emergency broadcast system.\n\nThis is a test of the emergency broadcast system.\n\n\n\nThe end of the EULA.";

					// Add 2nd step
					var secondStep = new Wizard.WizardStep ("Second Step");
					wizard.AddStep (secondStep);
					secondStep.HelpText = "This is the help text for the Second Step.\n\nPress the button demo changing the Title.\n\nIf First Name is empty the step will prevent moving to the next step.";

					View viewForControls = secondStep.Controls;
					ustring frameMsg = "Added to WizardStep.Controls";
					if (useStepView.Checked) {
						viewForControls = secondStep;
						frameMsg = "Added to WizardStep directly";
					}

					var buttonLbl = new Label () { Text = "Second Step Button: ", AutoSize = true, X = 1, Y = 1 };
					var button = new Button () {
						Text = "Press Me to Rename Step",
						X = Pos.Right (buttonLbl),
						Y = Pos.Top (buttonLbl)
					};
					button.Clicked += () => {
						secondStep.Title = "2nd Step";
						MessageBox.Query ("Wizard Scenario", "This Wizard Step's title was changed to '2nd Step'");
					};
					viewForControls.Add (buttonLbl, button);
					var lbl = new Label () { Text = "First Name: ", AutoSize = true, X = 1, Y = Pos.Bottom (buttonLbl) };
					var firstNameField = new TextField () { Text = "Number", Width = 30, X = Pos.Right (lbl), Y = Pos.Top (lbl) };
					viewForControls.Add (lbl, firstNameField);
					lbl = new Label () { Text = "Last Name:  ", AutoSize = true, X = 1, Y = Pos.Bottom (lbl) };
					var lastNameField = new TextField () { Text = "Six", Width = 30, X = Pos.Right (lbl), Y = Pos.Top (lbl) };
					viewForControls.Add (lbl, lastNameField);
					var thirdStepEnabledCeckBox = new CheckBox () { Text = "Enable Step _3", Checked = false, X = Pos.Left (lastNameField), Y = Pos.Bottom (lastNameField) };
					viewForControls.Add (thirdStepEnabledCeckBox);

					// Add a frame to demonstrate difference between adding controls to
					// WizardStep.Controls vs. WizardStep directly. This is here to demonstrate why 
					// adding to .Controls is preferred.
					var frame = new FrameView ($"A Broken Frame - {frameMsg}") {
						X = 0,
						Y = Pos.Bottom (thirdStepEnabledCeckBox) + 2,
						Width = Dim.Fill (),
						Height = 4,
						//ColorScheme = Colors.Error,
					};
					frame.Add (new TextField ("This is a TextField inside of the frame."));
					viewForControls.Add (frame);
					wizard.StepChanging += (args) => {
						if (args.OldStep == secondStep && firstNameField.Text.IsEmpty ) {
							args.Cancel = true;
							var btn = MessageBox.ErrorQuery ("Second Step", "You must enter a First Name to continue", "Ok");
						}
					};

					// Add 3rd (optional) step
					var thirdStep = new Wizard.WizardStep ("Third Step (Optional)");
					wizard.AddStep (thirdStep);
					thirdStep.HelpText = "This is step is optional (WizardStep.Enabled = false). Enable it with the checkbox in Step 2.";
					var step3Label = new Label () {
						Text = "This step is optional.",
						X = 0,
						Y = 0,
						AutoSize = true
					};
					thirdStep.Controls.Add (step3Label);
					var progLbl = new Label () { Text = "Third Step ProgressBar: ", AutoSize = true, X = 1, Y = 10 };
					var progressBar = new ProgressBar () {
						X = Pos.Right (progLbl),
						Y = Pos.Top (progLbl),
						Width = 40,
						Fraction = 0.42F
					};
					thirdStep.Controls.Add (progLbl, progressBar);
					thirdStep.Enabled = thirdStepEnabledCeckBox.Checked;
					thirdStepEnabledCeckBox.Toggled += (args) => {
						thirdStep.Enabled = thirdStepEnabledCeckBox.Checked;
					};
	
					// Add 4th step
					var fourthStep = new Wizard.WizardStep ("Step Four");
					wizard.AddStep (fourthStep);
					fourthStep.ShowHelp = false;
					var someText = new TextView () {
						Text = "This step (Step Four) shows how to hide the Help pane. The control pane contains this TextView (but it's hard to tell it's a TextView because of Issue #1800).",
						X = 0,
						Y = 0,
						Width = Dim.Fill (),
						Height = Dim.Fill (),
						WordWrap = true,
						AllowsTab = false
					};
					fourthStep.Controls.Add (someText);
					fourthStep.NextButtonText = "Go To Last Step";
					var scrollBar = new ScrollBarView (someText, true);

					scrollBar.ChangedPosition += () => {
						someText.TopRow = scrollBar.Position;
						if (someText.TopRow != scrollBar.Position) {
							scrollBar.Position = someText.TopRow;
						}
						someText.SetNeedsDisplay ();
					};

					scrollBar.VisibleChanged += () => {
						if (scrollBar.Visible && someText.RightOffset == 0) {
							someText.RightOffset = 1;
						} else if (!scrollBar.Visible && someText.RightOffset == 1) {
							someText.RightOffset = 0;
						}
					};

					someText.DrawContent += (e) => {
						scrollBar.Size = someText.Lines;
						scrollBar.Position = someText.TopRow;
						if (scrollBar.OtherScrollBarView != null) {
							scrollBar.OtherScrollBarView.Size = someText.Maxlength;
							scrollBar.OtherScrollBarView.Position = someText.LeftColumn;
						}
						scrollBar.LayoutSubviews ();
						scrollBar.Refresh ();
					};
					fourthStep.Controls.Add (scrollBar);

					// Add last step
					var lastStep = new Wizard.WizardStep ("The last step");
					wizard.AddStep (lastStep);
					lastStep.HelpText = "The wizard is complete!\n\nPress the Finish button to continue.\n\nPressing ESC will cancel the wizard.";
					var finalFinalStepEnabledCeckBox = new CheckBox () { Text = "Enable _Final Final Step", Checked = false, X = 0, Y = 1 };
					lastStep.Add (finalFinalStepEnabledCeckBox);

					// Add an optional FINAL last step
					var finalFinalStep = new Wizard.WizardStep ("The VERY last step");
					wizard.AddStep (finalFinalStep);
					finalFinalStep.HelpText = "This step only shows if it was enabled on the other last step.";
					finalFinalStep.Enabled = thirdStepEnabledCeckBox.Checked;
					finalFinalStepEnabledCeckBox.Toggled += (args) => {
						finalFinalStep.Enabled = finalFinalStepEnabledCeckBox.Checked;
					};

					Application.Run (wizard);

				} catch (FormatException) {
					actionLabel.Text = "Invalid Options";
				}
			};
			Win.Add (showWizardButton);

			Win.Add (actionLabel);
		}
	}
}
