# Candy AC IR

Candy AC IR is a Home Assistant custom component for controlling Candy air conditioners through an SLZB Ultima IR blaster.

The protocol was reverse-engineered from raw IR captures.

## Navigation

- [Repository root](../../../README.md)
- [Projects catalog](../../README.md)
- [Home](../README.md)

## Installation

1. Copy `custom_components/candy_ac_ir/` into the Home Assistant `<config>/custom_components/` directory.
2. Restart Home Assistant.
3. Open **Settings -> Devices & services -> Add integration**.
4. Search for **Candy AC IR**.
5. Configure:
   - **Name** - any friendly name, for example `Air Conditioner`.
   - **Host** - SLZB address, for example `http://slzb-ultima3.home.arpa`.
   - **IR endpoint** - `/api2?action=12` by default.
   - **Default temperature** - initial setpoint from 16 to 30 C.

## Features

| Feature | Status |
|---|---|
| Power on/off | Supported |
| Modes: auto, cool, heat, dry, fan | Supported |
| Temperature range 16-30 C | Supported |
| Fan speed: auto, low, medium, high | Supported |
| Swing | Supported |
| Presets: quiet, turbo, sleep | Supported |
| Timer | Available through the `candy_ac_ir.set_timer` service |

## Package Layout

```text
Byte  Value      Description
[0]   0xa6       Fixed device id
[1]   0xTJ       T=temperature-16, J=swing position
[3]   0xXX       Flags: 0x00=normal, 0x02=swing/eco, 0x40=timer
[4]   0x40/0x00  Power: on/off
[5]   0xXX       Fan: a0=auto, 60=low, 40=medium, 20=high
[6]   0xXX       Quiet/turbo: 00=normal, 40=turbo, 80=quiet
[7]   0xXX       Mode in bits 7:5 and timer hours in bits 6:0
[8]   0xXX       Timer minutes or sleep flag 0x80
[12]  0xXX       Command opcode
[13]  CS         Checksum: sum(bytes[0:13]) & 0xff
[14]  0xb7       Stop byte
```

## Raw IR Encoding

```text
Preamble: 3e 3c 3e 58 0b
Each payload bit: 0x21 for long/1 or 0x0b for short/0, followed by 0x0b padding
Trailer: 6 x 0x00 + 0xb7, encoded with the same bit scheme
Total: 5 + (22 bytes x 8 bits x 2) = 357 bytes
```
