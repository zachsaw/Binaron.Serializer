#!/bin/bash
set -e

if [ ! -f "$1" ]; then
    echo "Please specify path to dotCover.sh"
    exit 1
fi

echo "Running unit tests with code coverage report..."

rm -f /tmp/CoverageReport.html
rm -f dotCover.Output.dcvr
dotnet dotcover test -v n --dcFilters="-:type=Binaron.Serializer.IeeeDecimal.*;-:type=Binaron.Serializer.Tests.*"
$1 r DotCoverReportNix.xml
rm -f dotCover.Output.dcvr
open /tmp/CoverageReport.html
