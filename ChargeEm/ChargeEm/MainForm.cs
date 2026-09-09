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
        //  (Nothing; constructors have no explicit return value.)
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
        //  Fill the quote labels with the customer name, original risk factor, risk category,
        //    cost per $1,000, coverage amount, premium, discount, sales tax, and final annual
        //    total.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  riskFactor (double) - The original calculated risk factor to display and classify.
        //  costPerCoverage (double) - The premium multiplier for each dollar of policy
        //    coverage.
        //
        // RETURNS:
        //  (Nothing; void.)
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  annualPremium (double) - The initial annual premium before any discount or sales
        //    tax.
        //  coverageAmount (double) - The requested policy coverage, or zero if the coverage
        //    helper rejects the input.
        //  discountAmount (double) - The dollar discount returned by the selected discount
        //    helper; zero on a rejected discount input.
        //  premiumAfterDiscount (double) - The initial annual premium minus the discount
        //    amount.
        //  salesTaxAmount (double) - The configured sales tax applied to the premium after
        //    discount.
        //  totalAnnualPremium (double) - The premium after discount plus its sales tax.
        //
        // NOTES:
        //  Failed coverage or discount checks update the corresponding error label, but this
        //    method continues computing the remaining amounts using the helpers' out values.
        //  F2 displays two decimal places; C2 displays currency with two decimal places.
        //    Formatting does not round the stored calculation variables.
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

            lblTotalAnnualPremium.Text = "Invalid Data"; // default to an error message in case the coverage or discount calculations fail
            if(TryRefreshCoverageAmount(out double coverageAmount) == false)
                return;

            if(TryRefreshInitialAnnualPremium(coverageAmount, costPerCoverage, out double annualPremium) == false)
                return;

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
        //  Build the displayed customer name from the first-name and last-name inputs, trimming
        //    whitespace from the beginning and end of the combined text.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  (None)
        //
        // RETURNS:
        //  string - "(No Name Provided)" if both inputs are null or empty; otherwise the
        //    trimmed first name, a separating space, and last name.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // NOTES:
        //  Only the first and last name fields are used. Whitespace-only entries are not
        //    treated as empty by the initial IsNullOrEmpty checks.
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
        //  Choose the text label used to describe the calculated risk factor.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  riskFactor (double) - The original risk-factor value to classify.
        //
        // RETURNS:
        //  string - "Safe" when riskFactor is greater than or equal to zero; "Unsafe"
        //    otherwise.
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


        bool TryRefreshInitialAnnualPremium(double coverageAmount, double costPerCoverage, out double annualPremium)
        {
            if(TryCalculateInitialAnnualPremium(coverageAmount, costPerCoverage, out annualPremium) == true)
            {
                lblInitialAnnualPremium.Text = annualPremium.ToString("C2");
                return true;
            }
            else
            {
                lblInitialAnnualPremium.Text = "Invalid Coverage Amount";
                return false;
            }
        }


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
        //  Parse the policy coverage input and reject values that compare as zero or negative.
        //    Highlight the coverage input when it is rejected.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  coverageAmount (out double) - Receives the parsed coverage value on success or zero
        //    on failure.
        //
        // RETURNS:
        //  bool - False if parsing fails or coverageAmount is <= 0; true otherwise.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // NOTES:
        //  There is no separate finite-number check. NaN and positive infinity are not rejected
        //    by the <= 0 comparison.
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
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



        // METHOD NAME: TryCalculateInitialAnnualPremium
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Multiply the requested coverage by the per-dollar coverage multiplier to obtain the
        //    annual premium before discounts and sales tax.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  coverageAmount (double) - The requested policy coverage amount.
        //  costPerCoverage (double) - The premium multiplier for each dollar of coverage.
        //  annualPremium (out double) - Receives coverageAmount multiplied by costPerCoverage.
        //
        // RETURNS:
        //  bool - Always true in the current implementation.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // NOTES:
        //  Despite the Try prefix, this method performs no validation and has no false-return
        //    path.
        //
        // MODIFICATION HISTORY:
        // WHO     		WHEN         	WHAT
        // Millershaski 06 Sep 2026 	Initial Version
        bool TryCalculateInitialAnnualPremium(double coverageAmount, double costPerCoverage, out double annualPremium)
        {
            annualPremium = coverageAmount * costPerCoverage;
            return true;
        }



        // METHOD NAME: TryCalculateDiscountAmount
        // WRITTEN BY: Tyler J. Millershaski
        // DATE CREATED: 06 Sep 2026
        //
        // METHOD PURPOSE:
        //  Select the appropriate percentage or flat-dollar discount calculation according to
        //    the radio buttons. Keep the discount at zero when no discount is selected.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  annualPremium (double) - The initial annual premium before discounts or sales tax.
        //  discountAmount (out double) - Receives the selected dollar discount; remains zero
        //    for no discount or a rejected discount input.
        //
        // RETURNS:
        //  bool - True for no discount or a successful selected discount calculation; false if
        //    the selected discount helper rejects its input.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  discountType (int) - The selection code returned by GetDiscountType: 0 = none, 1 =
        //    percentage, 2 = flat amount.
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
        //  Read the percentage and flat-discount radio buttons and return the code identifying
        //    the selected discount calculation.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  (None)
        //
        // RETURNS:
        //  int - 1 for percentage discount, 2 for flat discount, or 0 when neither of those
        //    radio buttons is checked.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  (None)
        //
        // NOTES:
        //  The percentage option is checked first and therefore takes precedence if both radio
        //    buttons are checked.
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
        //  Parse the percentage input, reject values below zero or above 100, and convert an
        //    accepted percentage into a dollar discount on the initial annual premium.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  annualPremium (double) - The initial annual premium to which the percentage discount
        //    is applied.
        //  discountAmount (out double) - Receives the calculated dollar discount on success or
        //    zero when the input is rejected.
        //
        // RETURNS:
        //  bool - False if parsing fails or the percentage compares below 0 or above 100; true
        //    otherwise.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  percentageDiscount (double) - The entered percentage, where 5 means a five-percent
        //    discount.
        //
        // NOTES:
        //  For finite inputs, the accepted range is 0 through 100 inclusive. There is no
        //    explicit NaN check.
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
        //  Parse the flat-dollar discount input, reject negative values, and cap the accepted
        //    discount at the annual premium using Math.Min.
        //
        // PARAMETERS LIST (in Parameter Order):
        //  annualPremium (double) - The initial annual premium and maximum discount for a
        //    normal nonnegative premium.
        //  discountAmount (out double) - Receives the smaller of the entered flat discount and
        //    the annual premium; remains zero on rejected input.
        //
        // RETURNS:
        //  bool - False if parsing fails or the flat discount compares below zero; true
        //    otherwise.
        //
        // LOCAL VARIABLE DICTIONARY (in Alphabetical Order):
        //  flatDiscount (double) - The requested flat-dollar discount before it is capped at
        //    the annual premium.
        //
        // NOTES:
        //  This method does not separately validate annualPremium or reject non-finite inputs.
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
