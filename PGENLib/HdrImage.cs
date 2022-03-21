using System.Diagnostics;
using System;
using System.Text;

namespace PGENLib
{
    public class HdrImage
    {
        // attributi dell'immagine
        public static int width;
        public static int height;
        public Color[] pixels; // un vettore di tipo Color che contiene tutti i pixel

        // Costruttore: 
        public HdrImage(int width_constr, int height_constr, Color[] pixels)
        {
            width = width_constr;
            height = height_constr;
            this.pixels = pixels;
            Color col = new Color();
            // create an empty image
            for (int i = 0; i < width * height; i++)
            {
                this.pixels[i] = col;
            }
        }
        
        /// <summary>
        /// Verifica che date coordinate abbiano valori sensati, ovvero compresi tra 0 e il numero di righe/colonne
        /// </summary>
        private bool ValidCoord(int x, int y)
        {
            return (x > 0 && y > 0 && x < width && y < width);
        }

        /// <summary>
        /// Data una coppia di coordinate, restituisce la posizione del pixel nel vettore di memorizzazione.
        /// </summary>
        private int PixelOffset(int x, int y)
        {
            Debug.Assert(this.ValidCoord(x, y)) ;
            return y * width + x;
        }
        
        /// <summary>
        /// Imposta il colore di un pixel di date coordinate
        /// </summary>
        private void SetPixel(int x, int y, Color new_col)
        {
            Debug.Assert(this.ValidCoord(x, y));
            this.pixels[this.PixelOffset(x, y)] = new_col;
        }
        /// <summary>
        /// Data una coppia di coordinate, restituisce il colore del pixel corrispondente
        /// </summary>
        private Color GetPixel(int x, int y)
        {
            Debug.Assert(this.ValidCoord(x, y)) ;
            return this.pixels[this.PixelOffset(x,y)];
        }
        
        // =============== SEGUONO FUNZIONI PER LA LETTURA DA FILE ============
        
        // funzione grossa che legge i file ed è composta da 4 funzioncine
        // public void ReadPFMFile();

        /// <summary>
        /// funzione lettura di sequenza di 4 byte - CHI FINISCE PER PRIMO
        /// </summary>
        private float ReadFloat(Stream instr, double endianness)
        {
            byte[] bfloat = new byte[4] {0x00, 0x10, 0x20, 0x30};
            //bfloat = [ 0x00, 0x10, 0x20, 0x30];
            float ffloat = BitConverter.ToSingle(bfloat, 0);

            
            //for (int i = 0; i < 4; i++)
            //{
            //    float ffloat = ToSingle(byte[], 0);
            //}
            
            return ffloat;
        }

        /// <summary>
        /// funzione di lettura di byte fino a \n - MARTINO
        /// </summary>
        private string ReadLine(Stream str)
        {
            byte[] result = Encoding.ASCII.GetBytes(str);
        }
        
        /// <summary>
        /// funzione lettura dimensioni img - FRA
        /// </summary>
        private ParseImgSize()
        {
            return 0;
        }
        
        /// <summary>
        ///  Funzione che legge l'endianness e restituisce se è little o big.
        /// </summary>
        public static int ParseEndianness(double endianness)
        {
            int littleEnd = -1;
            int bigEnd = 1;
            Debug.Assert(endianness != 0); // il debug mi fa uscire il messaggio di errore solo se falsa.
            double End = endianness / Math.Abs(endianness); // endiannes = +/- 1
            if (End > 0)
            {
                return bigEnd;
            }
            else if (End < 0)
            {
                return littleEnd;
            }
            else return 0;
        }
        // DUBBIO: HO TRASFORMATO DA PRIVATE A PUBLIC PER VEDERE DAL MAIN SE FUNZIONA LA MIA PARTE
        // LA TERREI COMUNQUE PUBLIC.
    }

} 

    

