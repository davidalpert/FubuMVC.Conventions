include FileTest
require 'FileUtils'

fubumvc_root = File.join(File.dirname(__FILE__), 'lib', 'fubumvc')
fubumvc_sln = File.join(fubumvc_root,'src','FubuMVC.sln')
raise "Run `git submodule update --init` to populate the lib/fubumvc folder." unless File.exists? fubumvc_sln


require 'rubygems'
require 'zip/zip'
require 'zip/zipfilesystem'
require 'albacore'
require 'rexml/document'
include REXML

require "support/buildUtils.rb"
load "VERSION.txt"

ROOT_NAMESPACE = "FubuMVC.Conventions"
RESULTS_DIR = "build/test-reports"
BUILD_NUMBER_BASE = "0.1.0"
PRODUCT = ROOT_NAMESPACE
COPYRIGHT = 'Copyright Joshua Arnold 2011. All rights reserved.';
COMMON_ASSEMBLY_INFO = 'src/CommonAssemblyInfo.cs';
CLR_VERSION = "v4.0"
COMPILE_TARGET = "Debug"

props = { :archive => "build", :stage => "stage" }

desc "Compiles and runs unit tests"
task :all => [:default]

desc "**Default**, compiles and runs tests"
task :default => [:compile, :unit_tests, :release]

desc "Update the version information for the build"
assemblyinfo :version do |asm|
  tc_build_number = ENV["BUILD_NUMBER"]
  build_revision = tc_build_number || Time.new.strftime('5%H%M')
  BUILD_NUMBER = "#{BUILD_VERSION}.#{build_revision}"
  
  asm_version = BUILD_VERSION + ".0"
  
  begin
    commit = `git log -1 --pretty=format:%H`
  rescue
    commit = "git unavailable"
  end
  
  puts "##teamcity[buildNumber '#{BUILD_NUMBER}']" unless tc_build_number.nil?
  puts "Version: #{BUILD_NUMBER}" if tc_build_number.nil?
  asm.trademark = commit
  asm.product_name = PRODUCT
  asm.description = BUILD_NUMBER
  asm.version = asm_version
  asm.file_version = BUILD_NUMBER
  asm.custom_attributes :AssemblyInformationalVersion => asm_version
  asm.copyright = COPYRIGHT
  asm.output_file = COMMON_ASSEMBLY_INFO
end

desc "Prepares the working directory for a new build"
task :clean do	
	puts("recreating the build directory")
	buildDir = props[:archive]
	FileUtils.rm_r(Dir.glob(File.join(buildDir, '*')), :force=>true) if exists?(buildDir)
	FileUtils.rm_r(Dir.glob(buildDir), :force=>true) if exists?(buildDir)
	Dir.mkdir buildDir unless exists?(buildDir)
	Dir.mkdir RESULTS_DIR unless exists?(RESULTS_DIR)
end

desc "Compiles the app"
msbuild :compile => [:clean, :version, :compile_fubumvc] do |msb|

  msb.properties :configuration => COMPILE_TARGET
  msb.targets :Clean, :Build
  msb.solution = "src/#{ROOT_NAMESPACE}.sln"
end

task :compile_fubumvc do
	`pushd . && cd #{fubumvc_root} && git submodule update --init && rake compile && popd`
end

desc "Runs unit tests"
task :unit_tests do
  runner = NUnitRunner.new :compilemode => COMPILE_TARGET, :source => 'src', :platform => 'x86', :results => RESULTS_DIR
  runner.executeTests ['FubuMVC.Conventions.Tests']
end

task :release do
	copyOutputFiles "src/FubuMVC.Conventions/bin/#{COMPILE_TARGET}", "FubuMVC.Conventions*.{dll,pdb}", props[:archive]
	archive = Dir.glob(props[:archive])
	FileList["fubumvc.conventions.nuspec"].each do |spec|
      sh "lib/NuGet.exe pack #{spec} -o #{archive} -Version #{BUILD_NUMBER} -Symbols"
    end
end

def copyOutputFiles(fromDir, filePattern, outDir)
  Dir.glob(File.join(fromDir, filePattern)){|file| 		
	copy(file, outDir) if File.file?(file)
  } 
end
