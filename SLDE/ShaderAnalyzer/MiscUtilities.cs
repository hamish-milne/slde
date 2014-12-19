using System;

namespace SLDE.ShaderAnalyzer {
    static class MiscUtilities {
        public static int ComponentMaskToIndex(char c) {
            switch (c) {
                case 'r':
                case 'x':
                    return 0;
                case 'g':
                case 'y':
                    return 1;
                case 'b':
                case 'z':
                    return 2;
                case 'a':
                case 'w':
                    return 3;
                default:
                    throw new ArgumentException(String.Format("Character '{0}' is not a valid mask character.", c));
            }
        }

        public static char IndexToComponentMask(int i) {
            switch (i) {
                case 0:
                    return 'x';
                case 1:
                    return 'y';
                case 2:
                    return 'z';
                case 3:
                    return 'w';
                default:
                    throw new ArgumentException("Index must be within 0-3 range.");
            }
        }
    }
}
