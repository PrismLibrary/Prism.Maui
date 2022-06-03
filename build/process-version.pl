#!/usr/bin/perl

use strict;
use warnings;

opendir my $dir, "." or die "Cannot open directory: $!";

my @files = readdir $dir;
closedir $dir;

my $name = $ARGV[0];

my ($gitTag, $version, $preview);
for my $i (@files) {
    if($i =~ /$name\.((\d+\.\d+\.\d+)(\-[a-zA-Z0-9\-\.]+)?)\.nupkg$/) {
        $gitTag = $1;
        $version = $2;
        $preview = $3;
        last;
    }
}

if(not defined $gitTag or not defined $version) {
    die "Could not find version information";
}

if($preview eq "") {
    print "::set-output name=is-preview::false\n";
    print "::set-output name=release-display-name::$version\n";
} elsif($preview =~ m/alpha/) {
    print "::set-output name=is-preview::true\n";
    print "::set-output name=release-display-name::$version - Alpha\n";
} elsif($preview =~ m/beta/) {
    print "::set-output name=is-preview::true\n";
    print "::set-output name=release-display-name::$version - Beta\n";
} else {
    print "::set-output name=is-preview::true\n";
    print "::set-output name=release-display-name::$version - Preview\n";
}

print "::set-output name=version-name::$gitTag\n"
