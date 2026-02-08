module Car

type GasMaths = { Empty: bool; RemainingGas: float }

let drive distance gas =
    let remGas =
        if distance > 50 then gas / 2.0
        elif distance > 25 then gas - 10.0
        elif distance > 0 then gas - 1.0
        else gas

    { Empty = remGas <= 0
      RemainingGas = remGas }
