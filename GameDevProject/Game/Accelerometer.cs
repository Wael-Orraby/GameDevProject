using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameDevProject.Game
{
    /// <summary>
    /// Een statische encapsulatie van versnellingsmeterinvoer om spellen te voorzien van een op polling gebaseerd
    /// versnellingsmetersysteem.
    /// </summary>
    public static class Accelerometer
    {
        // we willen voorkomen dat de versnellingsmeter twee keer wordt geïnitialiseerd.
        private static bool isInitialized = false;

        // of de versnellingsmeter actief is of niet
        private static bool isActive = false;

        /// <summary>
        /// Initialiseert de versnellingsmeter voor het huidige spel. Deze methode kan slechts één keer per spel worden aangeroepen.
        /// </summary>
        public static void Initialize()
        {
            // zorg ervoor dat we de versnellingsmeter niet twee keer initialiseren
            if (isInitialized)
            {
                throw new InvalidOperationException("Initialize can only be called once");
            }

            // onthoud dat we geïnitialiseerd zijn
            isInitialized = true;
        }

        /// <summary>
        /// Geeft de huidige toestand van de versnellingsmeter terug.
        /// </summary>
        /// <returns>Een nieuwe VersnellingsmeterToestand met de huidige toestand van de versnellingsmeter.</returns>
        public static AccelerometerState GetState()
        {
            // zorg ervoor dat we de versnellingsmeter hebben geïnitialiseerd voordat we proberen de toestand op te halen
            if (!isInitialized)
            {
                throw new InvalidOperationException("You must Initialize before you can call GetState");
            }

            // maak een nieuwe waarde voor onze toestand
            Vector3 stateValue = new Vector3();

            return new AccelerometerState(stateValue, isActive);
        }
    }

    /// <summary>
    /// Een encapsulatie van de huidige toestand van de versnellingsmeter.
    /// </summary>
    public struct AccelerometerState
    {
        /// <summary>
        /// Geeft de huidige waarde van de versnellingsmeter weer in G-kracht.
        /// </summary>
        public Vector3 Acceleration { get; private set; }

        /// <summary>
        /// Geeft aan of de versnellingsmeter actief en actief is.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Initialiseert een nieuwe VersnellingsmeterToestand.
        /// </summary>
        /// <param name="acceleratie">De huidige versnelling (in G-kracht) van de versnellingsmeter.</param>
        /// <param name="isActief">Of de versnellingsmeter actief is of niet.</param>
        public AccelerometerState(Vector3 acceleration, bool isActive)
           : this()
        {
            Acceleration = acceleration;
            IsActive = isActive;
        }

        /// <summary>
        /// Geeft een string terug met de waarden van de Acceleratie en IsActief eigenschappen.
        /// </summary>
        /// <returns>Een nieuwe string die de toestand beschrijft.</returns>
        public override string ToString()
        {
            return string.Format("Acceleration: {0}, IsActive: {1}", Acceleration, IsActive);
        }
    }

}






