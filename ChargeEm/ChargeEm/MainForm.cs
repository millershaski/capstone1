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
        const double SALES_TAX_RATE = 0.06; // This is the rate for Michigan


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


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Subscribe to the KeyPress event for each TextBox to reset the warning when the user starts typing
            txtAge.KeyPress += ResetWarning;
            txtHeight.KeyPress += ResetWarning;
            txtWeight.KeyPress += ResetWarning;
            txtCoverageAmount.KeyPress += ResetWarning;
            txtPercentageDiscount.KeyPress += ResetWarning;
            txtFlatDiscount.KeyPress += ResetWarning;
        }



        void ResetWarning(object? sender, KeyPressEventArgs e)
        {
            if(sender != null && sender is TextBox textBox)
                textBox.BackColor = Color.White;
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
            ClearAllOutput(); // This method is called before generating a new quote to ensure that previous output is not incorrectly associated with the new quote.

            if(TryCalculateRiskFactor(out double riskFactor) == true)
            {
                double costPerCoverage = CalculateCostPerCoverage(riskFactor);
                RefreshOutput(riskFactor, costPerCoverage);
            }
        }



        // Clears all output fields on the form. 
        void ClearAllOutput()
        {
            string clearString = "-"; // A non-empty clear string tends to be better than an empty one.

            lblCustomerName.Text = clearString;
            lblRiskFactor.Text = clearString;
            lblRiskCategory.Text = clearString;
            lblCostPerThousand.Text = clearString;
            lblInitialAnnualPremium.Text = clearString;
            lblDiscountAmount.Text = clearString;
            lblPremiumAfterDiscount.Text = clearString;
            lblSalesTax.Text = clearString;
            lblTotalAnnualPremium.Text = clearString;
            lblCoverageAmount.Text = clearString;
        }



        // returns false if there's any invalid user input, true otherwise. If true, riskFactor is set to the calculated value.  If false, riskFactor is set to 0.
        bool TryCalculateRiskFactor(out double riskFactor)
        {
            riskFactor = 0;

            if(TryGetDoubleFromTextBox_DisplayError(txtAge, out double age) == false)
                return false;
            if(TryGetDoubleFromTextBox_DisplayError(txtHeight, out double height) == false)
                return false;
            if(TryGetDoubleFromTextBox_DisplayError(txtWeight, out double weight) == false)
                return false;

            // note that the following formula was provided per the specifications
            double numerator = age + Math.Sqrt((height * height) + (age * weight));
            double denominator = weight - (4.01d * age);
            riskFactor = numerator / denominator;

            return true;
        }



        // Returns true if the text in the specified TextBox can be successfully parsed as a double. If true, the parsed double value is assigned to the out parameter value.
        // Also displays feedback to the user if the TextBox's contents cannot be parsed into a double.
        bool TryGetDoubleFromTextBox_DisplayError(TextBox someTextBox, out double value)
        {
            if(TryGetDoubleFromTextBox(someTextBox, out value) == true)
                return true;

            DisplayInputError(someTextBox); 
            return false;
        }



        // Sets BackColor to LightPink and focuses on the TextBox. Note that a TextBox can subscribe to ResetWarning as appropriate (such as upon KeyPress) in order to automatically clear the displayed error
        void DisplayInputError(TextBox someTextBox)
        {
            someTextBox.BackColor = Color.LightPink;
            someTextBox.Focus();
        }



        // Returns true if the text in the specified TextBox can be successfully parsed as a double. If true, the parsed double value is assigned to the out parameter value.
        bool TryGetDoubleFromTextBox(TextBox someTextBox, out double value)
        {
            return double.TryParse(someTextBox.Text, out value);            
        }



        // Calculates the cost per coverage based on the provided risk factor. The formula for this calculation is provided in the specifications.
        double CalculateCostPerCoverage(double riskFactor)
        {
            if(Math.Abs(riskFactor) > 10.0)
            {
                // Get the leftmost digit and use it as the riskFactor
                while(Math.Abs(riskFactor) >= 10)
                {
                    riskFactor /= 10;
                }
            }

            return (10.1 - Math.Abs(riskFactor)) / 10d;
        }



        // Updates all output fields on the form based on the calculated risk factor and cost per 1K. 
        void RefreshOutput(double riskFactor, double costPerCoverage)
        {
            lblCustomerName.Text = GetCustomerName();
            lblRiskFactor.Text = riskFactor.ToString("F2");
            lblRiskCategory.Text = GetRiskCategoryLabel(riskFactor);
            lblCostPerThousand.Text = (costPerCoverage * 1000).ToString("C2"); // note that it's displayed to the user as "per 1000" so we multiply by 1000 to get the correct value to display

            if(TryGetCoverageAmount(out double coverageAmount) == true)
                lblCoverageAmount.Text = coverageAmount.ToString("C2");
            else
                lblCoverageAmount.Text = "Invalid Coverage Amount";

            if(TryCalculateInitialAnnualPremium(coverageAmount, costPerCoverage, out double annualPremium) == true)
                lblInitialAnnualPremium.Text = annualPremium.ToString("C2");
            else
                lblInitialAnnualPremium.Text = "Invalid Coverage Amount";

            if(TryCalculateDiscountAmount(annualPremium, out double discountAmount) == false)
                lblDiscountAmount.Text = "Invalid Discount Amount";
            else
                lblDiscountAmount.Text = discountAmount.ToString("C2");

            double premiumAfterDiscount = annualPremium - discountAmount;
            lblPremiumAfterDiscount.Text = premiumAfterDiscount.ToString("C2");

            double salesTaxAmount = premiumAfterDiscount * SALES_TAX_RATE;
            lblSalesTax.Text = salesTaxAmount.ToString("C2");

            double totalAnnualPremium = premiumAfterDiscount + salesTaxAmount;
            lblTotalAnnualPremium.Text = totalAnnualPremium.ToString("C2");
        }



        // Returns the full name of the customer based on the first and last name input fields. If both fields are empty, returns "(No Name Provided)".
        string GetCustomerName()
        {
            if(String.IsNullOrEmpty(txtFirstName.Text) && String.IsNullOrEmpty(txtLastName.Text))
                return "(No Name Provided)";

            return (txtFirstName.Text + " " + txtLastName.Text).Trim(); // trim to remove any leading or trailing whitespace in case either name is null or has leading/trailing whitespace
        }



        // Returns "Safe" if the riskFactor is positive (>= 0). Returns "Unsafe" otherwise
        string GetRiskCategoryLabel(double riskFactor)
        {
            if(riskFactor >= 0)
                return "Safe";
            else
                return "Unsafe";
        }



        bool TryGetCoverageAmount(out double coverageAmount)
        {
            if(TryGetDoubleFromTextBox(txtCoverageAmount, out coverageAmount) == false || coverageAmount <= 0)
            {
                coverageAmount = 0;
                DisplayInputError(txtCoverageAmount);
                return false;
            }
            return true;
        }



        // Attempts to calculate the initial (prior to any discounts or fees) annual premium based on the costPerCoverage and the allotted coverage amount. Returns false if the coverage amount is invalid (e.g., negative or zero), true otherwise. If true, the calculated annual premium is assigned to the out parameter annualPremium.
        bool TryCalculateInitialAnnualPremium(double coverageAmount, double costPerCoverage, out double annualPremium)
        { 
            annualPremium = coverageAmount * costPerCoverage;
            return true;
        }



        // Attempts to calculate the discount amount based on the annual premium and the selected discount type. Returns false if the inputted discount amount is invalid, true otherwise. If true, the calculated discount amount is assigned to the out parameter discountAmount.
        bool TryCalculateDiscountAmount(double annualPremium, out double discountAmount)
        {
            discountAmount = 0;

            int discountType = GetDiscountType();
            if(discountType == 1) // percentage
                return TryGetPercentageDiscountAmount(annualPremium, out discountAmount);            
            else if(discountType == 2) // flat amount
                return TryGetFlatDiscountAmount(annualPremium, out discountAmount);

            return true;
        }



        // Returns 0 if no discount type is selected, 1 if percentage discount is selected, and 2 if flat amount discount is selected.
        int GetDiscountType()
        {
            if(rdoPercentageDiscount.Checked == true)
                return 1;
            else if(rdoFlatDiscount.Checked == true)
                return 2;
            else
                return 0; // default to no discount if none are selected
        }



        bool TryGetPercentageDiscountAmount(double annualPremium, out double discountAmount)
        {
            discountAmount = 0;
            if(TryGetDoubleFromTextBox(txtPercentageDiscount, out double percentageDiscount) == false || percentageDiscount < 0 || percentageDiscount > 100)
            {
                DisplayInputError(txtPercentageDiscount);
                return false;
            }

            discountAmount = annualPremium * (percentageDiscount / 100d);
            return true;
        }



        bool TryGetFlatDiscountAmount(double annualPremium, out double discountAmount)
        {
            discountAmount = 0;
            if(TryGetDoubleFromTextBox(txtFlatDiscount, out double flatDiscount) == false || flatDiscount < 0)
            {
                DisplayInputError(txtFlatDiscount);
                return false;
            }

            discountAmount = Math.Min(flatDiscount, annualPremium); // ensures that the discount amount does not exceed the annual premium
            return true;
        }
    }
}
