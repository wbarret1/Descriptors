using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Descriptors
{
    [Flags]
    enum ELEMENTS
    {

        // The element will be the last four bits of the flag
        H  = 1,
        He = 2,
        Li = 3,
        Be = 4,
        B  = 5,
        C  = 6,
        N  = 7,
        O  = 8,
        F  = 9,
        Ne = 10,
        Na = 11,
        Mg = 12,
        Al = 13,
        Si = 14,
        P  = 15,
        S  = 16,
        Cl = 17,
        Ar = 18,
        K  = 19,
        Ca = 20,
        Sc = 21,
        Ti = 22,
        V  = 23,
        Cr = 24,
        Mn = 25,
        Fe = 26,
        Co = 27,
        Ni = 28,
        Cu = 29,
        Zn = 30,
        Ga = 31,
        Ge = 32,
        As = 33,
        Se = 34,
        Br = 35,
        Kr = 36,
        Rb = 37,
        Sr = 38,
        Y  = 39,
        Zr = 40,
        Nb = 41,
        Mo = 42,
        Tc = 43,
        Ru = 44,
        Rh = 45,
        Pd = 46,
        Ag = 47,
        Cd = 48,
        In = 49,
        Sn = 50,
        Sb = 51,
        Te = 52,
        I  = 53,
        Xe = 54,
        Cs = 55,
        Ba = 56,
        La = 57,
        Ce = 58,
        Pr = 59,
        Nd = 60,
        Pm = 61,
        Sm = 62,
        Eu = 63,
        Gd = 64,
        Tb = 65,
        Dy = 66,
        Ho = 67,
        Er = 68,
        Tm = 69,
        Yb = 70,
        Lu = 71,
        Hf = 72,
        Ta = 73,
        W  = 74,
        Re = 75,
        Os = 76,
        Ir = 77,
        Pt = 78,
        Au = 79,
        Hg = 80,
        Tl = 81,
        Pb = 82,
        Bi = 83,
        Po = 84,
        At = 85,
        Rn = 86,
        Fr = 87,
        Ra = 88,
        Ac = 89,
        Th = 90,
        Pa = 91,
        U  = 92,
        Np = 93,
        Pu = 94,
        Am = 95,
        Cm = 96,
        Bk = 97,
        Cf = 98,
        Es = 99,
        Fm = 100,
        Md = 101,
        No = 102,
        Lr = 103,
        Rf = 104,
        Db = 105,
        Sg = 106,
        Bh = 107,
        Hs = 108,
        Mt = 109,
        Ds = 110,
        Rg = 111,
        Cn = 112,
        Nh = 113,
        Fl = 114,
        Mc = 115,
        Lv = 116,
        Ts = 117,
        Og = 118,

        //Additional flags set here...
        // The lead bit will tell if the Element is aromatic.
        Aromatic = 0x8000
    };


    static class Element
    {
        static public string Symbol(ELEMENTS e)
        {
            return e.ToString();
        }

        static public bool ValidateSymbol(string symbol)
        {
            for (int i = 1; i < 119; i++)
            {
                if (((ELEMENTS)i).ToString() == symbol) return true;
            }
            return false;
        }

        static public ELEMENTS GetElementForSymbol(string symbol)
        {
            for (int i = 1; i < 119; i++)
            {
                if (((ELEMENTS)i).ToString() == symbol) return (ELEMENTS)i;
            }
            return (ELEMENTS)0;
        }

        static public string Name(ELEMENTS e)
        {
            if (e == ELEMENTS.H) return "Hydrogen";
            if (e == ELEMENTS.He) return "Helium";
            if (e == ELEMENTS.Li) return "Lithium";
            if (e == ELEMENTS.Be) return "Beryllium";
            if (e == ELEMENTS.B) return "Boron";
            if (e == ELEMENTS.C) return "Carbon";
            if (e == ELEMENTS.N) return "Nitrogen";
            if (e == ELEMENTS.O) return "Oxygen";
            if (e == ELEMENTS.F) return "Fluorine";
            if (e == ELEMENTS.Ne) return "Neon";
            if (e == ELEMENTS.Na) return "Sodium";
            if (e == ELEMENTS.Mg) return "Magnesium";
            if (e == ELEMENTS.Al) return "Aluminium";
            if (e == ELEMENTS.Si) return "Silicon";
            if (e == ELEMENTS.P) return "Phosphorus";
            if (e == ELEMENTS.S) return "Sulfur";
            if (e == ELEMENTS.Cl) return "Chlorine";
            if (e == ELEMENTS.Ar) return "Argon";
            if (e == ELEMENTS.K) return "Potassium";
            if (e == ELEMENTS.Ca) return "Calcium";
            if (e == ELEMENTS.Sc) return "Scandium";
            if (e == ELEMENTS.Ti) return "Titanium";
            if (e == ELEMENTS.V) return "Vanadium";
            if (e == ELEMENTS.Cr) return "Chromium";
            if (e == ELEMENTS.Mn) return "Manganese";
            if (e == ELEMENTS.Fe) return "Iron";
            if (e == ELEMENTS.Co) return "Cobalt";
            if (e == ELEMENTS.Ni) return "Nickel";
            if (e == ELEMENTS.Cu) return "Copper";
            if (e == ELEMENTS.Zn) return "Zinc";
            if (e == ELEMENTS.Ga) return "Gallium";
            if (e == ELEMENTS.Ge) return "Germanium";
            if (e == ELEMENTS.As) return "Arsenic";
            if (e == ELEMENTS.Se) return "Selenium";
            if (e == ELEMENTS.Br) return "Bromine";
            if (e == ELEMENTS.Kr) return "Krypton";
            if (e == ELEMENTS.Rb) return "Rubidium";
            if (e == ELEMENTS.Sr) return "Strontium";
            if (e == ELEMENTS.Y) return "Yttrium";
            if (e == ELEMENTS.Zr) return "Zirconium";
            if (e == ELEMENTS.Nb) return "Niobium";
            if (e == ELEMENTS.Mo) return "Molybdenum";
            if (e == ELEMENTS.Tc) return "Technetium";
            if (e == ELEMENTS.Ru) return "Ruthenium";
            if (e == ELEMENTS.Rh) return "Rhodium";
            if (e == ELEMENTS.Pd) return "Palladium";
            if (e == ELEMENTS.Ag) return "Silver";
            if (e == ELEMENTS.Cd) return "Cadmium";
            if (e == ELEMENTS.In) return "Indium";
            if (e == ELEMENTS.Sn) return "Tin";
            if (e == ELEMENTS.Sb) return "Antimony";
            if (e == ELEMENTS.Te) return "Tellurium";
            if (e == ELEMENTS.I) return "Iodine";
            if (e == ELEMENTS.Xe) return "Xenon";
            if (e == ELEMENTS.Cs) return "Caesium";
            if (e == ELEMENTS.Ba) return "Barium";
            if (e == ELEMENTS.La) return "Lanthanum";
            if (e == ELEMENTS.Ce) return "Cerium";
            if (e == ELEMENTS.Pr) return "Praseodymium";
            if (e == ELEMENTS.Nd) return "Neodymium";
            if (e == ELEMENTS.Pm) return "Promethium";
            if (e == ELEMENTS.Sm) return "Samarium";
            if (e == ELEMENTS.Eu) return "Europium";
            if (e == ELEMENTS.Gd) return "Gadolinium";
            if (e == ELEMENTS.Tb) return "Terbium";
            if (e == ELEMENTS.Dy) return "Dysprosium";
            if (e == ELEMENTS.Ho) return "Holmium";
            if (e == ELEMENTS.Er) return "Erbium";
            if (e == ELEMENTS.Tm) return "Thulium";
            if (e == ELEMENTS.Yb) return "Ytterbium";
            if (e == ELEMENTS.Lu) return "Lutetium";
            if (e == ELEMENTS.Hf) return "Hafnium";
            if (e == ELEMENTS.Ta) return "Tantalum";
            if (e == ELEMENTS.W) return "Tungsten";
            if (e == ELEMENTS.Re) return "Rhenium";
            if (e == ELEMENTS.Os) return "Osmium";
            if (e == ELEMENTS.Ir) return "Iridium";
            if (e == ELEMENTS.Pt) return "Platinum";
            if (e == ELEMENTS.Au) return "Gold";
            if (e == ELEMENTS.Hg) return "Mercury";
            if (e == ELEMENTS.Tl) return "Thallium";
            if (e == ELEMENTS.Pb) return "Lead";
            if (e == ELEMENTS.Bi) return "Bismuth";
            if (e == ELEMENTS.Po) return "Polonium";
            if (e == ELEMENTS.At) return "Astatine";
            if (e == ELEMENTS.Rn) return "Radon";
            if (e == ELEMENTS.Fr) return "Francium";
            if (e == ELEMENTS.Ra) return "Radium";
            if (e == ELEMENTS.Ac) return "Actinium";
            if (e == ELEMENTS.Th) return "Thorium";
            if (e == ELEMENTS.Pa) return "Protactinium";
            if (e == ELEMENTS.U) return "Uranium";
            if (e == ELEMENTS.Np) return "Neptunium";
            if (e == ELEMENTS.Pu) return "Plutonium";
            if (e == ELEMENTS.Am) return "Americium";
            if (e == ELEMENTS.Cm) return "Curium";
            if (e == ELEMENTS.Bk) return "Berkelium";
            if (e == ELEMENTS.Cf) return "Californium";
            if (e == ELEMENTS.Es) return "Einsteinium";
            if (e == ELEMENTS.Fm) return "Fermium";
            if (e == ELEMENTS.Md) return "Mendelevium";
            if (e == ELEMENTS.No) return "Nobelium";
            if (e == ELEMENTS.Lr) return "Lawrencium";
            if (e == ELEMENTS.Rf) return "Rutherfordium";
            if (e == ELEMENTS.Db) return "Dubnium";
            if (e == ELEMENTS.Sg) return "Seaborgium";
            if (e == ELEMENTS.Bh) return "Bohrium";
            if (e == ELEMENTS.Hs) return "Hassium";
            if (e == ELEMENTS.Mt) return "Meitnerium";
            if (e == ELEMENTS.Ds) return "Darmstadtium";
            if (e == ELEMENTS.Rg) return "Roentgenium";
            if (e == ELEMENTS.Cn) return "Copernicium";
            if (e == ELEMENTS.Nh) return "Nihonium";
            if (e == ELEMENTS.Fl) return "Flerovium";
            if (e == ELEMENTS.Mc) return "Moscovium";
            if (e == ELEMENTS.Lv) return "Livermorium";
            if (e == ELEMENTS.Ts) return "Tennessine";
            if (e == ELEMENTS.Og) return "Oganesson";
            return string.Empty;
        }
    }
}