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
        const double SALES_TAX_RATE = 0.06;


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
            {
                double costPer1K = CalculateCostPer1K(riskFactor);
                RefreshOutput(riskFactor, costPer1K);
            }
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



        // Calculates the cost per 1K based on the provided risk factor.  The formula for this calculation is provided in the specifications.
        double CalculateCostPer1K(double riskFactor)
        {
            if(Math.Abs(riskFactor) > 10.0)
            {
                // Get the leftmost digit and use it as the riskFactor
                while(Math.Abs(riskFactor) >= 10)
                {
                    riskFactor /= 10;
                }
            }

            return (10.1 - riskFactor) * (1d / 10d);
        }



        // Updates all output fields on the form based on the calculated risk factor and cost per 1K.  This method is called after a successful calculation of the risk factor and cost per 1K.
        void RefreshOutput(double riskFactor, double costPer1K)
        {
            lblRiskFactor.Text = riskFactor.ToString("F2");
            lblRiskCategory.Text = GetRiskCategoryLabel(riskFactor);
            lblCostPerThousand.Text = costPer1K.ToString("C2");

            if(TryCalculateInitialAnnualPremium(costPer1K, out double annualPremium) == true)
                lblInitialAnnualPremium.Text = annualPremium.ToString("C2");            
            else
                lblInitialAnnualPremium.Text = "Invalid Coverage Amount";

            if(TryCalculateDiscountAmount(annualPremium, out double discountAmount) == false)
                lblDiscountAmount.Text = "Invalid Discount Amount";
            else
                lblDiscountAmount.Text = discountAmount.ToString("C2");

            double premiumAfterDiscount = annualPremium - discountAmount;
            lblPremiumAfterDiscount.Text = premiumAfterDiscount.ToString("C2");
            lblSalesTax.Text = SALES_TAX_RATE.ToString("P2");

            double totalAnnualPremium = premiumAfterDiscount * (1 + SALES_TAX_RATE);
            lblTotalAnnualPremium.Text = totalAnnualPremium.ToString("C2");
        }



        // Returns "Safe" if the riskFactor is positive (>= 0). Returns "Unsafe" otherwise
        string GetRiskCategoryLabel(double riskFactor)
        {
            if(riskFactor >= 0)
                return "Safe";
            else
                return "Unsafe";
        }



        // Attempts to calculate the initial (prior to any discounts or fees) annual premium based on the costPer1K and the allotted coverage amount. Returns false if the coverage amount is invalid (e.g., negative or zero), true otherwise. If true, the calculated annual premium is assigned to the out parameter annualPremium.
        bool TryCalculateInitialAnnualPremium(double costPer1K, out double annualPremium)
        {
            if(TryGetDoubleFromTextBox(txtCoverageAmount, out double coverageAmount) == false || coverageAmount <= 0)
            {
                annualPremium = 0;
                return false;
            }

            annualPremium = (coverageAmount / 1000d) * costPer1K; // note that the coverage amount is divided by 1000 to convert it to units of 1K
            return true;
        }



        // Attempts to calculate the discount amount based on the annual premium and the selected discount type. Returns false if the inputted discount amount is invalid, true otherwise. If true, the calculated discount amount is assigned to the out parameter discountAmount.
        bool TryCalculateDiscountAmount(double annualPremium, out double discountAmount)
        {
            discountAmount = 0;

            int discountType = GetDiscountType();
            if(discountType == 1) // percentage
            {
                if(TryGetDoubleFromTextBox(txtPercentageDiscount, out double percentageDiscount) == false || percentageDiscount < 0)
                    return false;

                discountAmount = annualPremium * (percentageDiscount / 100d);
            }
            else if(discountType == 2) // flat amount
            {
                if(TryGetDoubleFromTextBox(txtFlatDiscount, out double flatDiscount) == false || flatDiscount < 0)
                    return false;

                discountAmount = flatDiscount;
                return true;
            }

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
    }
}
