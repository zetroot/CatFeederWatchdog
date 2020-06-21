using System;
using System.Collections.Generic;
using System.Text;

namespace EPaperDriver
{
    /// <summary>
    /// Команды дисплея
    /// </summary>
    public enum EinkDisplayCommand
    {
        /// <summary>
        /// Panel Setting (PSR) (R00H)
        /// </summary>
        PanelSetting = 0x00,
        /// <summary>
        /// Power Setting (PWR) (R01H)
        /// </summary>
        PowerSetting = 0x01,
        /// <summary>
        /// Power OFF (POF) (R02H)
        /// </summary>
        PowerOff = 0x02,
        /// <summary>
        /// Power OFF Sequence Setting(PFS) (R03H)
        /// </summary>
        PowerOffSequenceSetting = 0x03,
        /// <summary>
        /// Power ON (PON) (R04H)
        /// </summary>
        PowerOn = 0x04,
        /// <summary>
        /// Booster Soft Start (BTST) (R06H)
        /// </summary>
        BoosterSoftStart = 0x06,
        /// <summary>
        /// Deep sleep (DSLP) (R07H)
        /// </summary>
        DeepSleep = 0x07,
        /// <summary>
        /// Data Start Transmission 1 (DTM1) (R10H)
        /// </summary>
        DataStartTransmission1 = 0x10,
        /// <summary>
        /// Data stop (DSP) (R11H)
        /// </summary>
        DataStop = 0x11,
        /// <summary>
        /// Display Refresh (DRF) (R12H)
        /// </summary>
        DisplayRefresh = 0x12,
        /// <summary>
        /// Data Start Transmission 2 (DTM2)
        /// </summary>
        DataStartTransmission2 = 0x13,
        /// <summary>
        /// Partial Display Refresh (PDRF)  0x16
        /// </summary>
        PartialDisplayRefresh = 0x16,
        /// <summary>
        /// VCOM LUT (LUTC) (R20H)
        /// </summary>
        LutForVcom = 0x20,
        /// <summary>
        /// Black LUT (LUTB) (R21H)
        /// </summary>
        LutBlue = 0x21,
        /// <summary>
        /// White LUT(LUTW) (R22H)
        /// </summary>
        LutWhite = 0x22,
        /// <summary>
        /// Gray1 LUT (LUTG1) (R23H)
        /// </summary>
        LutGray1 = 0x23,
        /// <summary>
        /// Gray2 LUT (LUTG2) (R24H)
        /// </summary>
        LutGray2 = 0x24,
        /// <summary>
        /// Red0 LUT (LUTR0) (R25H)
        /// </summary>
        LutRed0 = 0x25,
        /// <summary>
        /// Red1 LUT (LUTR1) (R26H)
        /// </summary>
        LutRed1 = 0x26,
        /// <summary>
        /// Red2 LUT (LUTR2) (R27H)
        /// </summary>
        LutRed2 = 0x27,
        /// <summary>
        /// Red3 LUT (LUTR3) (R28H)
        /// </summary>
        LutRed3 = 0x28,
        /// <summary>
        /// XON LUT (LUTXON) (R29H)
        /// </summary>
        LutXon = 0x29,
        /// <summary>
        /// PLL Control (PLL) (R30H)
        /// </summary>
        PllControl = 0x30,
        /// <summary>
        /// Temperature Sensor Calibration(TSC) (R40H)
        /// </summary>
        TemperatureSensorCommand = 0x40,
        /// <summary>
        /// Temperature Sensor Internal/External(TSE) (R41H)
        /// </summary>
        TemperatureCalibration = 0x41,
        /// <summary>
        /// Temperature Sensor Write (TSW) (R42H)
        /// </summary>
        TemperatureSensorWrite = 0x42,
        /// <summary>
        /// Temperature Sensor Read (TSR) (R43H)
        /// </summary>
        TemperatureSensorRead = 0x43,
        /// <summary>
        /// VCOM and Data Interval Setting(CDI) (R50H)
        /// </summary>
        VcomAndDataIntervalSetting = 0x50,
        /// <summary>
        /// Low Power Detection(LPD) (R51h)
        /// </summary>
        LowPowerDetection = 0x51,
        /// <summary>
        /// TCON Setting(TCON) (R60h)
        /// </summary>
        TconSetting = 0x60,
        /// <summary>
        /// Resolution Setting(TRES) (R61H)
        /// </summary>
        TconResolution = 0x61,
        /// <summary>
        /// SPI Flash Control(DAM) (R65H)
        /// </summary>
        SpiFlashControl = 0x65,
        /// <summary>
        /// Revision(REV) (R70H)
        /// </summary>
        Revision = 0x70,
        /// <summary>
        /// Get status(FLG) (R71H)
        /// </summary>
        GetStatus = 0x71,
        /// <summary>
        /// Auto measure vcom(AMV) (R80h)
        /// </summary>
        AutoMeasurementVcom = 0x80,
        /// <summary>
        /// VCOM Value(VV) (R81h)
        /// </summary>
        ReadVcomValue = 0x81,
        /// <summary>
        /// VCOM-DC Setting(VDCS) (R82H)
        /// </summary>
        VcmDcSetting = 0x82,
        /// <summary>
        /// Flash Mode (RE5H)
        /// </summary>
        FlashMode = 0xE5,

        PowerOptimization = 0xF8
    }
}
