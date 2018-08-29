using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtonPlanCheck
{
    public class DefaultCalcOptions : Dictionary<string, string>
    {
        public DefaultCalcOptions()
        {
            this.Add("General/CalculationGridSizeInCM", "0.25");
            //this.Add("General/DoseCalculationGridCutOffInCM", "3");
            //this.Add("General/FluenceCalculationGridCutOffInCM", "1.5");
            //this.Add("General/LateralCutOffInUnitsOfSigma", "3");
            //this.Add("General/LowDoseCutOffInPercents", "0.5");
            this.Add("ModulatedScanning/Beamline/OverwriteSpacingBetweenScanningLines", "FALSE");
            this.Add("ModulatedScanning/Beamline/OverwriteSpotSpacingInScanningDirection", "FALSE");
            this.Add("ModulatedScanning/Beamline/SpacingBetweenScanningLinesInCM", "0.5");
            this.Add("ModulatedScanning/Beamline/SpotSpacingInScanningDirectionInCM", "0.5");
            //this.Add("ModulatedScanning/Optimizer/DosePercentageOnTargetBorder", "0.99");
            //this.Add("ModulatedScanning/Optimizer/FallOffParameterInCM", "0.5");
            //this.Add("ModulatedScanning/Optimizer/MarginAroundTargetUsedAsConstraint", "0.8");
            //this.Add("ModulatedScanning/Optimizer/OptimizationMethod", "SimultaneousSpotOptimization");
            this.Add("ModulatedScanning/Optimizer/OptimizeOnlyForConstraints", "FALSE");
            this.Add("ModulatedScanning/Optimizer/SmoothMostDistalSpotInBeamlet", "FALSE");
            //this.Add("ModulatedScanning/Optimizer/SpotSmoothingRadiusInCM", "2");
            //this.Add("ModulatedScanning/Optimizer/SpotSmoothingWeight", "0");
            //this.Add("ModulatedScanning/Optimizer/StopCriterion", "1E-08");
            //this.Add("ModulatedScanning/PostProcessing/SpotInterpolationFactorInScanningDirection", "1");
            //this.Add("ModulatedScanning/PostProcessing/SpotTruncationThreshold", "0.5");
        }
    }
}
