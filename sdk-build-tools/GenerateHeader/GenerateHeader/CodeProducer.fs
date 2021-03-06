﻿module CodeProducer

open FSharp.Data
open System.IO

/// Tracks indent level and writes an indent to a StreamWriter.
type Indent = {
    level : int
}
with
    member i.Indent() = { level = i.level + 1 }
    member i.Unindent() =
        let newLevel = i.level - 1
        if newLevel < 0 then
            failwith "Unindent caused a negative indent"
        { level = newLevel }

    member i.Width = i.level * 4
    member i.WriteIndent(stream : TextWriter) =

        for i = 0 to i.level - 1 do
            stream.Write("    ")


/// Information about the type being generated.
type TypeInfo = {
    moduleName : string
    typeName : string
    description : string
    notes : string
}

/// Indicates whether the field is a scalar or vector;
/// vectors only have one dimension.
type FieldCategory =
| Scalar
| Vector of size : int

/// Types recognized by the code generator.
type FieldType =
| Uint16
| Uint32
| UInt64
| Int32
| Float32
| Float64
| String

let elementSize = function
    | Uint16  -> 2
    | Uint32  -> 4
    | UInt64  -> 8
    | Int32   -> 4
    | Float32 -> 4
    | Float64 -> 8
    | String  -> 1


/// Describes a field.
type FieldInfo = {
    name : string
    typ : FieldType
    fieldCat : FieldCategory
    description : string
    note : string
    obsoleteNote : string
}
with
    member fi.IsObsolete = fi.obsoleteNote.Length > 0
    member fi.StorageSize =
        let multiplier = match fi.fieldCat with
                         | Scalar -> 1
                         | Vector size -> size
        multiplier * elementSize fi.typ

type FilePart =
| FileBegin of TypeInfo
| FileEnd of TypeInfo
| NamespaceBegin of TypeInfo
| NamespaceEnd of TypeInfo
| ModuleBegin of TypeInfo
| ModuleEnd of TypeInfo
| TypeBegin of TypeInfo
| TypeEnd of TypeInfo
| Fields of FieldInfo array
| FieldOffsets of TypeInfo * FieldInfo array

type FileName = string
type Modifier = string
type CodeProducer = FileName -> TextWriter -> Modifier -> Indent -> FilePart -> Indent
