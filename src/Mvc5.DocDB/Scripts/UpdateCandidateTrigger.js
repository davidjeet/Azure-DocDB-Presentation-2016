// Copyright (c) Microsoft Corporation.  All rights reserved.

// Register DocDB JavaScript server API for intelisense: 
//   either add the file to Tools->Options->Text Editor->JavaScript->Intellisense->References and reference the group registered 
//   or provide path to the file explicitly.
/// <reference group="Generic" />
/// <reference path="C:\Program Files (x86)\Microsoft Visual Studio 12.0\JavaScript\References\DocDbWrapperScript.js" />

/**
* This script runs as a trigger:
* for each inserted document, look at document.size and update aggregate properties of metadata document: minSize, maxSize, totalSize.
*/
function updateCandidateTrigger() {
    // HTTP error codes sent to our callback funciton by DocDB server.
    var ErrorCode = {
        RETRY_WITH: 449,
    }

    var collection = getContext().getCollection();
    var collectionLink = collection.getSelfLink();

    // Get the document from request (the script runs as trigger, thus the input comes in request).
    var doc = getContext().getRequest().getBody();

    if (doc.size > 0) {
        getAndUpdateMetadata();

    }

    function getAndUpdateMetadata() {
            doc.Revisions = doc.Revisions + 1;        
            collection.replaceDocument(doc._self, doc);
    }

}