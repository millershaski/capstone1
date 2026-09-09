// FILENAME: MainForm.cs
// WRITTEN BY: Tyler J. Millershaski
// DATE CREATED: 06 Sep 2026 
// 
// PART OF PROJECT: ChargeEm
// 
// FILE PURPOSE:
//  This file contains the main form and its associated processing logic.
// 
// CLASS NAME: MainForm
//
// CLASS PURPOSE:
//  Handle the input and output of the insurance quotation form. The designer
//  partial class supplies the visual controls; this file contains their event
//  handling, calculation helpers, display formatting, and input-warning behavior.
//
// CLASS CONSTANT DICTIONARY (in Alphabetical Order):
//  SALES_TAX_RATE (double) - The 0.06 tax multiplier used by this assignment,
//                           applied to the premium after discount.
//
// CLASS VARIABLE DICTIONARY (in Alphabetical Order):
//  (None declared in this file.) The form's control fields are supplied by the
//  designer partial class. The class constant is documented separately above.
//
// MODIFICATION HISTORY:
// WHO              WHEN         WHAT
// Millershaski   06 Sep 2026  Initial Version
// --- -------- -------------------------------------------------
namespace ChargeEm
{
    public partial class MainForm : Form
    {
        const double SALES_TAX_RATE = 0.06; // This is the rate for Michigan


        // METHOD NAME: MainForm (CONSTRUCTOR)
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Construct the main form and initialize the controls and layout defined in the
        //    designer partial class.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  (None)
        //
        // RETURNS:
        //  (Nothing)
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        public MainForm()
        {
            InitializeComponent();
        }



        // METHOD NAME: OnLoad
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Run the inherited form-load behavior, then connect each numeric input text box to
        //    the handler that clears its warning color when the user types.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  e (EventArgs) - Event information supplied when the form loads.
        //
        // RETURNS:
        //  (Nothing; void.)
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
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



        // METHOD NAME: ResetWarning
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Restore the sending text box to a white background when it raises a KeyPress event.
        //    This clears the visual warning without validating the new contents.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  sender (object?) - The control that raised the event. Null or non-TextBox senders are ignored.
        //  e (KeyPressEventArgs) - Information about the keypress (not used) 
        //
        // RETURNS:
        //  (Nothing; void.)
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  textBox (TextBox) - The sender after the pattern match confirms that it is a textbox.
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        void ResetWarning(object? sender, KeyPressEventArgs e)
        {
            if(sender != null && sender is TextBox textBox)
                textBox.BackColor = Color.White;
        }



        // METHOD NAME: OnGenerateQuoteClick
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Clear the previous quote, attempt to calculate the customer risk factor, and
        //    generate the new quote output if the measurement values can be parsed.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  sender (object?) - The control that raised the click event (not used)
        //  e (EventArgs) - Click event information (not used)
        //
        // RETURNS:
        //  (Nothing; void.)
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  costPerCoverage (double) - The multiplier per dollar of requested policy coverage.
        //  riskFactor (double) - The risk factor returned through TryCalculateRiskFactor.
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        void OnGenerateQuoteClick(object? sender, EventArgs e)
        {
            ClearAllOutput(); // This method is called before generating a new quote to ensure that previous output is not incorrectly associated with the new quote.
            lblTotalAnnualPremium.Text = "Invalid Data"; // default to an error message in case the coverage or discount calculations fail

            if(TryCalculateRiskFactor(out double riskFactor) == true)
            {
                double costPerCoverage = CalculateCostPerCoverage(riskFactor);
                RefreshOutput(riskFactor, costPerCoverage);
            }
        }



        // METHOD NAME: ClearAllOutput
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Replace every quote output value with a dash so that previous quote details are not
        //    displayed as part of the current quote. Input text boxes are not cleared.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  (None)
        //
        // RETURNS:
        //  (Nothing; void.)
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  clearString (string) - The dash used as the placeholder for all cleared output labels.
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
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



        // METHOD NAME: TryCalculateRiskFactor
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Parse the age, height, and weight inputs, then calculate the risk factor.
        //    (Highlights the first input TextBox that cannot be parsed.)
        //
        // PARAMETERS LIST (in Parameter Order):
        //  riskFactor (out double) - Stores the calculated risk factor upon success. Set to zero if any input parsing fails.
        //
        // RETURNS:
        //  bool - False when any input cannot be parsed. True otherwise.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  age (double) - The customer age in years, parsed from txtAge.
        //  denominator (double) - The bottom portion of the provided formula.
        //  height (double) - The customer height in inches, parsed from txtHeight.
        //  numerator (double) - The top portion of the provided formula.
        //  weight (double) - The customer weight in pounds, parsed from txtWeight.        
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        bool TryCalculateRiskFactor(out double riskFactor)
        {
            riskFactor = 0;

            if(TryGetDoubleFromTextBox_DisplayError(txtAge, out double age, false) == false)
                return false;
            if(TryGetDoubleFromTextBox_DisplayError(txtHeight, out double height, false) == false || height <= 0)
                return false;            
            if(TryGetDoubleFromTextBox_DisplayError(txtWeight, out double weight, false) == false)
                return false;

            // note that the following formula was provided per the specifications
            double numerator = age + Math.Sqrt((height * height) + (age * weight));
            double denominator = weight - (4.01d * age);

            if(denominator == 0) // prevent division by zero
                return false;

            riskFactor = numerator / denominator;
            return true;
        }



        // METHOD NAME: TryGetDoubleFromTextBox_DisplayError
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Attempt to parse the specified text box as a double and highlight that text box upon fail
        //
        // PARAMETERS LIST (in Parameter Order):
        //  allowNegative (bool) - True to accept negative values. False to reject them.
        //  someTextBox (TextBox) - The input control whose Text property will be parsed.
        //  value (out double) - Stores the parsed value on success. Set to 0 upon fail
        //
        // RETURNS:
        //  bool - True if parsing succeeds and the value is within the allowed range. False otherwise.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        bool TryGetDoubleFromTextBox_DisplayError(TextBox someTextBox, out double value, bool allowNegative)
        {
            if(TryGetDoubleFromTextBox(someTextBox, out value) == true)
            {
                if(allowNegative == true || value > 0)              
                    return true;
            }
            DisplayInputError(someTextBox);
            return false;
        }



        // METHOD NAME: DisplayInputError
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Mark an input text box with a light-pink background and request keyboard focus so
        //    the user can correct its contents.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  someTextBox (TextBox) - The input control to highlight and focus.
        //
        // RETURNS:
        //  (Nothing; void.)
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // NOTES:
        //  A text box subscribed to ResetWarning can restore its white background on the next
        //    KeyPress event.
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        void DisplayInputError(TextBox someTextBox)
        {
            someTextBox.BackColor = Color.LightPink;
            someTextBox.Focus();
        }



        // METHOD NAME: TryGetDoubleFromTextBox
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Attempt to convert the text box contents to a double (default culture).
        //
        // PARAMETERS LIST (in Parameter Order):
        //  someTextBox (TextBox) - The TextBox containing the numeric text.
        //  value (out double) - Stores the parsed number on success. Stores zero upon fail.
        //
        // RETURNS:
        //  bool - The success or failure result returned by double.TryParse.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        // 
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        bool TryGetDoubleFromTextBox(TextBox someTextBox, out double value)
        {
            return double.TryParse(someTextBox.Text, out value);
        }



        // METHOD NAME: CalculateCostPerCoverage
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Calculates the per-dollar coverage multiplier based on the normalized risk factor. The formula is (10.1 - absolute normalized risk factor) / 10.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  riskFactor (double) - The previously calculated risk factor.
        //
        // RETURNS:
        //  double - The per-dollar coverage multiplier 
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)        
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        double CalculateCostPerCoverage(double riskFactor)
        {
            if(Math.Abs(riskFactor) > 10.0) // note that a riskFactor of Abs(10.0) will not be divided
            {
                // Divide until only 1 digit remains (per specification)
                while(Math.Abs(riskFactor) >= 10)
                {
                    riskFactor /= 10;
                }
            }

            return (10.1 - Math.Abs(riskFactor)) / 10d;
        }



        // METHOD NAME: RefreshOutput
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Display the customer details and calculated quote amounts. Stop if any coverage, premium, or discount refresh fails.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  riskFactor (double) - The original calculated risk factor to display.
        //  costPerCoverage (double) - The multiplier per dollar of policy coverage.
        //
        // RETURNS:
        //  (Nothing; void.)
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  annualPremium (double) - The annual premium before discounts and sales tax.
        //  coverageAmount (double) - The requested policy coverage amount.
        //  discountAmount (double) - The selected discount in dollars.
        //  premiumAfterDiscount (double) - The annual premium minus the discount amount.
        //  salesTaxAmount (double) - The sales tax on the premium after discount.
        //  totalAnnualPremium (double) - The premium after discount plus sales tax.
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        void RefreshOutput(double riskFactor, double costPerCoverage)
        {
            lblCustomerName.Text = GetCustomerName();
            lblRiskFactor.Text = riskFactor.ToString("F2");
            lblRiskCategory.Text = GetRiskCategoryLabel(riskFactor);
            lblCostPerThousand.Text = (costPerCoverage * 1000).ToString("C2"); // note that it's displayed to the user as "per 1000" so we multiply by 1000 to get the correct value to display

            if(TryRefreshCoverageAmount(out double coverageAmount) == false)
                return;

            RefreshInitialAnnualPremium(coverageAmount, costPerCoverage, out double annualPremium);
            if(TryRefreshDiscountAmount(annualPremium, out double discountAmount) == false)
                return;

            double premiumAfterDiscount = annualPremium - discountAmount;
            lblPremiumAfterDiscount.Text = premiumAfterDiscount.ToString("C2");

            double salesTaxAmount = premiumAfterDiscount * SALES_TAX_RATE;
            lblSalesTax.Text = salesTaxAmount.ToString("C2");

            double totalAnnualPremium = premiumAfterDiscount + salesTaxAmount;
            lblTotalAnnualPremium.Text = totalAnnualPremium.ToString("C2");
        }



        // METHOD NAME: GetCustomerName
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Returns the full customer name from the first name, middle initial, and last name.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  (None)
        //
        // RETURNS:
        //  string - "(No Name Provided)" if the first and last names are blank. Otherwise, the full name.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  fullName (string) - The combined customer name before the final trim.
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        string GetCustomerName()
        {
            if(String.IsNullOrWhiteSpace(txtFirstName.Text) && String.IsNullOrWhiteSpace(txtLastName.Text)) // if both last name and first name are blank, return a default string (even if middle initial is populated)
                return "(No Name Provided)";

            string fullName = "";
            if(String.IsNullOrWhiteSpace(txtFirstName.Text) == false)
                fullName += txtFirstName.Text.Trim();
            if(String.IsNullOrWhiteSpace(txtMiddleInitial.Text) == false)
                fullName += " " + txtMiddleInitial.Text.Trim();
            if(String.IsNullOrWhiteSpace(txtLastName.Text) == false)
                fullName += " " + txtLastName.Text.Trim();

            return fullName.Trim(); // trim to remove any leading or trailing whitespace in case either name is null or has leading/trailing whitespace
        }



        // METHOD NAME: GetRiskCategoryLabel
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Get the "safe" or "unsafe label for the calculated risk factor.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  riskFactor (double) - The calculated risk factor.
        //
        // RETURNS:
        //  string - "Safe" if riskFactor is greater than or equal to zero. "Unsafe" otherwise.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        string GetRiskCategoryLabel(double riskFactor)
        {
            if(riskFactor >= 0)
                return "Safe";
            else
                return "Unsafe";
        }



        // METHOD NAME: TryRefreshCoverageAmount
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Attempt to read the policy coverage amount and display it as currency.
        //    Displays an error message upon fail.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  coverageAmount (out double) - Stores the coverage amount on success. Set to zero upon fail.
        //
        // RETURNS:
        //  bool - True if the coverage input is accepted. False otherwise.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        bool TryRefreshCoverageAmount(out double coverageAmount)
        {
            if(TryGetCoverageAmount(out coverageAmount) == true)
            {
                lblCoverageAmount.Text = coverageAmount.ToString("C2");
                return true;
            }
            else
            {
                lblCoverageAmount.Text = "Invalid Coverage Amount";
                return false;
            }
        }



        // METHOD NAME: TryRefreshInitialAnnualPremium
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Calculates the initial annual premium and display it as currency.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  coverageAmount (double) - The requested policy coverage amount.
        //  costPerCoverage (double) - The multiplier per dollar of policy coverage.
        //  annualPremium (out double) - Stores coverageAmount multiplied by costPerCoverage.
        //
        // RETURNS:
        //  (Nothing; void.)
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        void RefreshInitialAnnualPremium(double coverageAmount, double costPerCoverage, out double annualPremium)
        {
            annualPremium = coverageAmount * costPerCoverage;
            lblInitialAnnualPremium.Text = annualPremium.ToString("C2");
        }



        // METHOD NAME: TryRefreshDiscountAmount
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Attempt to calculate the selected discount and display it as currency.
        //    Display an error message upon fail.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  annualPremium (double) - The annual premium before discounts and sales tax.
        //  discountAmount (out double) - Stores the discount in dollars. Set to zero for no discount or upon fail.
        //
        // RETURNS:
        //  bool - True if the discount calculation succeeds or no discount is selected. False otherwise.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        bool TryRefreshDiscountAmount(double annualPremium, out double discountAmount)
        {
            if(TryCalculateDiscountAmount(annualPremium, out discountAmount) == true)
            {
                lblDiscountAmount.Text = discountAmount.ToString("C2");
                return true;
            }
            else
            {
                lblDiscountAmount.Text = "Invalid Discount Amount";
                return false;
            }
        }



        // METHOD NAME: TryGetCoverageAmount
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Attempt to parse the policy coverage amount.
        //    Reject zero or negative values and highlight the TextBox upon fail.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  coverageAmount (out double) - Stores the parsed coverage amount on success. Set to zero upon fail.
        //
        // RETURNS:
        //  bool - False if parsing fails or the coverage amount is less than or equal to zero. True otherwise.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        bool TryGetCoverageAmount(out double coverageAmount)
        {
            if(TryGetDoubleFromTextBox(txtCoverageAmount, out coverageAmount) == false || coverageAmount <= 0) // note that the 0 fails and not just negative values
            {
                coverageAmount = 0;
                DisplayInputError(txtCoverageAmount);
                return false;
            }
            return true;
        }



        // METHOD NAME: TryCalculateDiscountAmount
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Calculate the percentage or flat dollar discount selected by the radio buttons.
        //    Use zero when no discount is selected.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  annualPremium (double) - The annual premium before discounts and sales tax.
        //  discountAmount (out double) - Stores the discount in dollars. Set to zero for no discount or upon fail.
        //
        // RETURNS:
        //  bool - False if the selected discount input is rejected. True otherwise.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  discountType (int) - The selected discount: 0 = none, 1 = percentage, 2 = flat amount.
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
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



        // METHOD NAME: GetDiscountType
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Get the selected discount type from the radio buttons and converts it to an integer.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  (None)
        //
        // RETURNS:
        //  int - 1 for a percentage discount, 2 for a flat discount, or 0 for no discount.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        int GetDiscountType()
        {
            if(rdoPercentageDiscount.Checked == true)
                return 1;
            else if(rdoFlatDiscount.Checked == true)
                return 2;
            else
                return 0; // default to no discount if none are selected
        }



        // METHOD NAME: TryGetPercentageDiscountAmount
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Attempt to parse the percentage discount and calculate its dollar amount.
        //    Reject percentages below 0 or above 100 and highlight the TextBox upon fail.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  annualPremium (double) - The annual premium before discounts and sales tax.
        //  discountAmount (out double) - Stores the calculated dollar discount on success. Set to zero upon fail.
        //
        // RETURNS:
        //  bool - False if parsing fails or the percentage is below 0 or above 100. True otherwise.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  percentageDiscount (double) - The entered discount percentage (5 means 5%).
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
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



        // METHOD NAME: TryGetFlatDiscountAmount
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Attempt to parse the flat dollar discount. Reject negative values and highlight the TextBox upon fail.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  annualPremium (double) - The annual premium before discounts and sales tax, also used as the maximum discount.
        //  discountAmount (out double) - Stores the smaller of the entered discount and the annual premium. Set to zero upon fail.
        //
        // RETURNS:
        //  bool - False if parsing fails or the flat discount is below zero. True otherwise.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  flatDiscount (double) - The entered flat dollar discount before applying the premium limit.
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
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
