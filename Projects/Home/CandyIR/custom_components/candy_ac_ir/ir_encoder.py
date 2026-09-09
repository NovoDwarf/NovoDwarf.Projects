"""
Candy AC IR Protocol Encoder
Reverse-engineered from SLZB Ultima raw captures.

Packet structure (15 bytes, MSB-first, every-other-bit in raw stream):
  [0]  0xa6          Device ID (fixed)
  [1]  0xTJ          T = temp-16 (high nibble), J = jalousie pos (low nibble)
  [2]  0x00          Reserved
  [3]  0xXX          Special flags: 0x00=normal, 0x02=jalousie/eco, 0x40=timer
  [4]  0x40/0x00     Power: 0x40=ON, 0x00=OFF
  [5]  0xXX          Fan: 0xa0=auto, 0x60=low, 0x40=med, 0x20=high  (+0x08=sleep)
  [6]  0xXX          Silent/Turbo: 0x00=normal, 0x40=turbo, 0x80=silent
  [7]  0xXX          Mode (top 3 bits) + timer hours (low 7 bits)
                     mode: 0x00=auto, 0x20=cool, 0x40=dry, 0x80=heat
  [8]  0xXX          Timer minutes (0-59) OR 0x80=sleep flag
  [9]  0x80/0x00     0x80 = auto/cool context
  [10] 0x00          Reserved
  [11] 0x00          Reserved
  [12] 0xXX          Command opcode (see OPCODE_* constants)
  [13] CS            Checksum = sum(bytes[0:13]) & 0xff
  [14] 0xb7          Stop byte (fixed)

Raw encoding:
  Preamble: 5 bytes (e.g. 3e 3c 3e 58 0b)
  Each payload bit -> (0x21 if 1 else 0x0b) + 0x0b padding
  Trailer: 6×0x00 + 0xb7 encoded same way
"""

# Jalousie low-nibble values
JALOUSIE_AUTO   = 0x2   # Default / auto (no fixed position)
JALOUSIE_SWING  = 0xC   # Auto-swing (wave up/down)
JALOUSIE_UP     = 0x1   # Fixed position 1 (up)
JALOUSIE_MID    = 0x2   # Fixed position 2 (middle) — same as auto nibble
JALOUSIE_DOWN   = 0x3   # Fixed position 3 (down)
JALOUSIE_OFF    = 0x0   # Off / power-off state

# Fan speed byte[5] values
FAN_AUTO  = 0xA0
FAN_LOW   = 0x60
FAN_MED   = 0x40
FAN_HIGH  = 0x20

# Mode byte[7] top-3-bits values
MODE_AUTO = 0x00
MODE_COOL = 0x20
MODE_DRY  = 0x40
MODE_HEAT = 0x80

# Silent/Turbo byte[6]
SILENT_NORMAL = 0x00
SILENT_TURBO  = 0x40
SILENT_QUIET  = 0x80

# Command opcodes byte[12]
OPCODE_TEMP      = 0x00  # Temperature / default
OPCODE_SWING     = 0x02  # Jalousie swing toggle
OPCODE_FAN       = 0x04  # Fan speed change
OPCODE_POWER_OFF = 0x05  # Power off
OPCODE_MODE      = 0x06  # Mode change
OPCODE_ECO       = 0x07  # Eco mode (СПЕЦ.ФУНКЦИИ)
OPCODE_SILENT    = 0x08  # Silent/Turbo
OPCODE_SLEEP     = 0x0B  # Comfort sleep
OPCODE_TIMER     = 0x10  # Timer
OPCODE_JALOUSIE  = 0x1F  # Jalousie fixed position

PREAMBLE = bytes.fromhex("3e3c3e580b")


def _checksum(payload: list[int]) -> int:
    return sum(payload[:13]) & 0xFF


def _build_payload(
    temp: int,
    power: bool,
    mode: int,
    fan: int,
    jalousie_nibble: int,
    silent_turbo: int,
    opcode: int,
    byte3: int = 0x00,
    byte8: int = 0x00,
    byte9: int = 0x00,
    timer_hours: int = 0,
) -> bytes:
    """Assemble 15-byte payload."""
    b1 = ((temp - 16) << 4) | (jalousie_nibble & 0x0F)
    b4 = 0x40 if power else 0x00
    b7 = mode | (timer_hours & 0x7F)
    payload = [
        0xA6,       # [0] device ID
        b1,         # [1] temp + jalousie
        0x00,       # [2] reserved
        byte3,      # [3] special flags
        b4,         # [4] power
        fan,        # [5] fan speed
        silent_turbo,  # [6] silent/turbo
        b7,         # [7] mode + timer hours
        byte8,      # [8] timer minutes / sleep flag
        byte9,      # [9] context flag
        0x00,       # [10] reserved
        0x00,       # [11] reserved
        opcode,     # [12] command opcode
    ]
    payload.append(_checksum(payload))  # [13]
    payload.append(0xB7)                # [14]
    return bytes(payload)


def _encode_raw(payload: bytes) -> str:
    """Convert 15-byte payload to SLZB raw hex string."""
    # Append trailer: 6×0x00 + 0xb7
    frame = payload + bytes(6) + bytes([0xB7])

    bits = []
    for byte in frame:
        for i in range(7, -1, -1):
            bits.append((byte >> i) & 1)

    raw = list(PREAMBLE)
    for bit in bits:
        raw.append(0x21 if bit else 0x0B)
        raw.append(0x0B)

    return "".join(f"{b:02x}" for b in raw)


# ─── Public API ──────────────────────────────────────────────────────────────

def cmd_power_on(temp: int = 22, mode: int = MODE_AUTO,
                 fan: int = FAN_AUTO, jalousie: int = JALOUSIE_AUTO) -> str:
    """Turn on with given settings."""
    byte9 = 0x80 if mode in (MODE_AUTO, MODE_COOL) else 0x00
    opcode = OPCODE_MODE if mode != MODE_AUTO else OPCODE_TEMP
    payload = _build_payload(
        temp=temp, power=True, mode=mode, fan=fan,
        jalousie_nibble=jalousie, silent_turbo=SILENT_NORMAL,
        opcode=opcode, byte9=byte9,
    )
    return _encode_raw(payload)


def cmd_power_off(temp: int = 22, fan: int = FAN_AUTO) -> str:
    """Turn off (sends current state with power=OFF)."""
    payload = _build_payload(
        temp=temp, power=False, mode=MODE_AUTO, fan=fan,
        jalousie_nibble=JALOUSIE_OFF, silent_turbo=SILENT_NORMAL,
        opcode=OPCODE_POWER_OFF,
    )
    return _encode_raw(payload)


def cmd_set_temp(temp: int, mode: int = MODE_AUTO,
                 fan: int = FAN_AUTO, power: bool = True) -> str:
    """Set temperature."""
    byte9 = 0x80 if mode in (MODE_AUTO, MODE_COOL) else 0x00
    payload = _build_payload(
        temp=temp, power=power, mode=mode, fan=fan,
        jalousie_nibble=JALOUSIE_AUTO, silent_turbo=SILENT_NORMAL,
        opcode=OPCODE_TEMP, byte9=byte9,
    )
    return _encode_raw(payload)


def cmd_set_fan(fan: int, temp: int = 22, mode: int = MODE_AUTO) -> str:
    """Set fan speed."""
    byte9 = 0x80 if mode in (MODE_AUTO, MODE_COOL) else 0x00
    payload = _build_payload(
        temp=temp, power=True, mode=mode, fan=fan,
        jalousie_nibble=JALOUSIE_AUTO, silent_turbo=SILENT_NORMAL,
        opcode=OPCODE_FAN, byte9=byte9,
    )
    return _encode_raw(payload)


def cmd_set_mode(mode: int, temp: int = 22, fan: int = FAN_AUTO) -> str:
    """Set operation mode."""
    byte9 = 0x80 if mode in (MODE_AUTO, MODE_COOL) else 0x00
    payload = _build_payload(
        temp=temp, power=True, mode=mode, fan=fan,
        jalousie_nibble=JALOUSIE_AUTO, silent_turbo=SILENT_NORMAL,
        opcode=OPCODE_MODE, byte9=byte9,
    )
    return _encode_raw(payload)


def cmd_silent(temp: int = 22, mode: int = MODE_HEAT, fan: int = FAN_LOW) -> str:
    """Enable silent (quiet) mode."""
    payload = _build_payload(
        temp=temp, power=True, mode=mode, fan=fan,
        jalousie_nibble=JALOUSIE_AUTO, silent_turbo=SILENT_QUIET,
        opcode=OPCODE_SILENT, byte9=0x80,
    )
    return _encode_raw(payload)


def cmd_turbo(temp: int = 22, mode: int = MODE_HEAT, fan: int = FAN_LOW) -> str:
    """Enable turbo (power) mode."""
    payload = _build_payload(
        temp=temp, power=True, mode=mode, fan=fan,
        jalousie_nibble=JALOUSIE_AUTO, silent_turbo=SILENT_TURBO,
        opcode=OPCODE_SILENT, byte9=0x80,
    )
    return _encode_raw(payload)


def cmd_sleep(temp: int = 22, fan: int = FAN_AUTO) -> str:
    """Enable comfort sleep mode."""
    fan_sleep = fan | 0x08  # set sleep bit in fan byte
    payload = _build_payload(
        temp=temp, power=True, mode=MODE_AUTO, fan=fan_sleep,
        jalousie_nibble=JALOUSIE_AUTO, silent_turbo=SILENT_NORMAL,
        opcode=OPCODE_SLEEP, byte8=0x80, byte9=0x80,
    )
    return _encode_raw(payload)


def cmd_jalousie_swing(temp: int = 22, mode: int = MODE_AUTO,
                       fan: int = FAN_LOW) -> str:
    """Toggle jalousie auto-swing."""
    payload = _build_payload(
        temp=temp, power=True, mode=mode, fan=fan,
        jalousie_nibble=JALOUSIE_SWING, silent_turbo=SILENT_NORMAL,
        opcode=OPCODE_SWING, byte9=0x80,
    )
    return _encode_raw(payload)


def cmd_jalousie_position(position: int, temp: int = 22,
                          mode: int = MODE_AUTO, fan: int = FAN_LOW) -> str:
    """
    Set jalousie to fixed position.
    position: 1=up, 2=middle, 3=down
    """
    nibble_map = {1: JALOUSIE_UP, 2: JALOUSIE_MID, 3: JALOUSIE_DOWN}
    nibble = nibble_map.get(position, JALOUSIE_AUTO)
    payload = _build_payload(
        temp=temp, power=True, mode=mode, fan=fan,
        jalousie_nibble=nibble, silent_turbo=SILENT_NORMAL,
        opcode=OPCODE_JALOUSIE, byte3=0x02, byte9=0x80,
    )
    return _encode_raw(payload)


def cmd_timer_off(minutes: int, temp: int = 22, mode: int = MODE_HEAT,
                  fan: int = FAN_LOW) -> str:
    """
    Set timer to turn OFF after N minutes (max 24h).
    minutes: 30..1440 in steps of 30 for <12h, 60 for >12h
    """
    hours = minutes // 60
    mins  = minutes % 60
    payload = _build_payload(
        temp=temp, power=False, mode=mode, fan=fan,
        jalousie_nibble=JALOUSIE_AUTO, silent_turbo=SILENT_NORMAL,
        opcode=OPCODE_TIMER, byte3=0x40,
        timer_hours=hours, byte8=mins, byte9=0x80,
    )
    return _encode_raw(payload)


def cmd_eco(temp: int = 22, mode: int = MODE_HEAT, fan: int = FAN_LOW) -> str:
    """Enable ECO / СПЕЦ.ФУНКЦИИ mode."""
    payload = _build_payload(
        temp=temp, power=True, mode=mode, fan=fan,
        jalousie_nibble=JALOUSIE_SWING, silent_turbo=SILENT_NORMAL,
        opcode=OPCODE_ECO, byte3=0x02, byte9=0x80,
    )
    return _encode_raw(payload)
