# Breakasm

Как можно более простой и тупой ассемблер для генереации кода.

Для запуска:

```
Breakasm test.asm test.prg
```

PRG файл всегда 64 Кбайта (размером с адресное пространство 6502). Текущий указатель ассемблирования (`ORG`) можно устанавливать в любое место PRG.

## Синтакс

Исходный текст разбивается на строки следующего формата:

```
[LABEL:]  COMMAND  [OPERAND1, OPERAND2, OPERAND3]       ; Comments
```

Метка (`LABEL`) является необязательной. Команда (`COMMAND`) содержит инструкцию 6502 или одну из директив ассебмлера. Операнды зависят от команды.

## Встроенные директивы

|Директива|Описание|
|---|---|
|ORG|Установить текущее положение ассемблирования в PRG.|
|DEFINE|Определить простую константу|
|BYTE|Вывести байт или строку|
|WORD|Вывести uint16_t в порядке little-endian. Можно использовать как простые числа, так и метки и адреса.|
|END|Завершить ассемблирование|
|PROCESSOR|Определяет тип процессора в информативных целях|

## Пример исходника

Чтобы много не писать, просто покажу пример исходника. Делайте так же и всё должно получиться.

```asm
; Test program

LABEL1:

    PROCESSOR 6502
;    ORG     $100

    DEFINE  KONST   #5

    LDX     KONST

AGAIN:
    NOP
    LDA     SOMEDATA, X         ; Load some data
    JSR     ADDSOME             ; Call sub
    STA     $12, X
    CLC
    BCC     AGAIN               ; Test branch logic

ADDSOME:                        ; Test ALU
    ADC     KONST
    PHP                         ; Test flags in/out
    PLP
    RTS

    ASL     A

SOMEDATA:
    BYTE    12, $FF, "Hello, world" 
    WORD    AGAIN

END
```