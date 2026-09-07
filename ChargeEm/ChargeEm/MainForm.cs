// FILENAME: MainForm.cs
// WRITTEN BY: Tyler J. Millershaski
// DATE CREATED: 06 Sep 2026 (SEE VERSION CONTROL)
// 
// PART OF PROJECT: ChargeEm
// 
// FILE PURPOSE:
//  This file contains the main form and its associated processing logic.
// 
// CLASS NAME: MainForm
//
// CLASS PURPOSE:
// This is the code-behind portion of the main application form.  This file 
// contains the main form for the entire application.  All user input is gathered 
// on this form.  The calculations which are performed by the application reside 
// in this file as well.  Finally all generated output is contained here too.
//      
// CLASS VARIABLE DICTIONARY (in Alphabetical Order):
//  
// MODIFICATION HISTORY:
// WHO   WHEN   WHAT
// --- -------- -------------------------------------------------
namespace ChargeEm
{
    public partial class MainForm : Form
    {
        //  METHOD NAME: MainForm
        //  
        //  METHOD PURPOSE:
        //   This is the main entry point for the application start up.
        //  	 
        //  PARAMETERS LIST (in Parameter Order):
        //   (None)
        //  	
        //  RETURNS:
        //   (Nothing)
        //  	
        //  LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //   (None)
        //  
        // MODIFICATION HISTORY:
        // WHO   WHEN   WHAT
        // --- -------- -------------------------------------------------
        public MainForm()
        {
            InitializeComponent();
        }



        //  METHOD NAME: OnGenerateQuoteClick
        //  
        //  METHOD PURPOSE:
        //   This method is the event handler for the Generate Quote button click event.  It is responsible for gathering user input, performing calculations, and generating the output quote.
        //  	 
        //  PARAMETERS LIST (in Parameter Order):
        //   object? sender - The source of the event (the button that was clicked).
        //   EventArgs e - The event data associated with the button click event.
        //  	
        //  RETURNS:
        //   (Nothing)
        //  	
        //  LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //   (None)
        //  
        // MODIFICATION HISTORY:
        // WHO   WHEN   WHAT
        // --- -------- -------------------------------------------------
        void OnGenerateQuoteClick(object? sender, EventArgs e)
        {
            if(TryCalculateRiskFactor(out double riskFactor) == true)
                UpdateRiskFactorOutput(riskFactor);
        }



        // returns false if there's any invalid user input, true otherwise. If true, riskFactor is set to the calculated value.  If false, riskFactor is set to 0.
        bool TryCalculateRiskFactor(out double riskFactor)
        {
            riskFactor = 0;
            
            if(TryGetDoubleFromTextBox(txtAge, out double age) == false)
                return false;
            if(TryGetDoubleFromTextBox(txtHeight, out double height) == false)
                return false;
            if(TryGetDoubleFromTextBox(txtWeight, out double weight) == false)
                return false;

            // note that the following formula was provided per the specifications
            double numerator = age + Math.Sqrt((height * height) + (age * weight));
            double denominator = weight - (4.01f * age);
            riskFactor = numerator / denominator;

            return true;
        }


        // Returns true if the text in the specified TextBox can be successfully parsed as a double. If true, the parsed double value is assigned to the out parameter value.
        bool TryGetDoubleFromTextBox(TextBox someTextBox, out double value)
        {
            return double.TryParse(someTextBox.Text, out value);
        }



        void UpdateRiskFactorOutput(double riskFactor)
        {
            uu
        }
    }
}
