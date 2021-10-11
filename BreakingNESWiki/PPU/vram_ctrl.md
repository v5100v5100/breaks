# Контроллер VRAM

Схема представляет собой "вспомогательный мозг" нижней части PPU для управления интерфейсом VRAM.

## Транзисторная схема

<img src="/BreakingNESWiki/imgstore/vram_control_tran.jpg" width="1000px">

Анатомически схема поделена на 2 большие половинки, левая больше связана с управляющим сигналом WR_internal, а правая с RD_internal.
В состав каждой половинки входит RS-триггер и линия задержки, которая автоматически устанавливает триггер.

Схема выдает наружу ряд контрольных линий:
- RD (RD_internal): на выход /RD
- WR (WR_internal): на выход /WR
- /ALE: на выход ALE (ALE=1, когда шина AD работает как адресная, ALE=0, когда AD работает как шина данных)
- TSTEP: на схему DATAREAD, позволяет TV/TH счетчикам выполнить инкремент
- DB/PAR: на схему DATAREAD, соединяет внутреннюю шину PPU DB с псевдорегистром PAR (PPU address register)
- PD/RB: соединяет внешнюю шину PPU с read buffer-ом, для загрузки в него нового значения
- TH/MUX: предварительное название. Направить регистр TH на выход MUX, в результате чего это значение уйдет на color-buffer и предположительно в палитру.
- XRB: включает tri-state логику, которая отсоединяет PPU read buffer от внутренней шины данных.

## Логическая схема

<img src="/BreakingNESWiki/imgstore/vram_control_logisim.jpg" width="1000px">

Чтобы сказать что-то более конкретное, нужно вначале разобрать остальные узлы PPU.

TBD.

## Отличие PAL PPU

Отличий в схеме не обнаружено.

<img src="/BreakingNESWiki/imgstore/vram_control_pal.jpg" width="1000px">

Поверхность исследуемой микросхемы в этом месте была немного грязновата, но я пометил все ключевые контрольные линии, поэтому нет сомнений, что схема идентична NTSC PPU.

## Симуляция

TBD.