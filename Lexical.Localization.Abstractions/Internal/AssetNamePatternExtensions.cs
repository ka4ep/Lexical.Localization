// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           22.10.2018
// Url:            http://lexical.fi
// --------------------------------------------------------
namespace Lexical.Localization
{
    public static partial class AssetNamePatternExtensions
    {
        /// <summary>
        /// A name pattern such as "{section_0/}{section_1/}{section_n/}{Key}" is matched to occurance indexes 0 first, then 1 and then 'n'.
        /// This method moves last numeric match, for example '1' to 'n'. 
        /// </summary>
        /// <param name="match"></param>
        /// <returns>match</returns>
        public static IAssetNamePatternMatch _fixPartsWithOccurancesAndLastOccurance(this IAssetNamePatternMatch match)
        {
            foreach (var kp in match.Pattern.ParameterMap)
            {
                // Get parts
                IAssetNamePatternPart[] array = kp.Value;
                // Has more than one
                if (array == null || array.Length < 2) continue;
                IAssetNamePatternPart lastPart = array[array.Length - 1];
                // Assert that if last part is last occurance part
                if (lastPart.OccuranceIndex != int.MaxValue) continue;
                // Assert that it is empty
                if (match.PartValues[lastPart.CaptureIndex] != null) continue;

                for (int j = array.Length - 1; j >= 0; j--)
                {
                    // Get part
                    IAssetNamePatternPart part = array[j];
                    // Assert that part is not last occurance part
                    if (part.OccuranceIndex == int.MaxValue) continue;
                    // Get value
                    int ix = array[j].CaptureIndex;
                    string occuranceValue = match.PartValues[ix];
                    if (occuranceValue == null) continue;
                    // Swap values
                    match.PartValues[lastPart.CaptureIndex] = occuranceValue;
                    match.PartValues[ix] = null;
                    // We are done for this parameter.
                    break;
                }
            }
            return match;
        }

    }
}
