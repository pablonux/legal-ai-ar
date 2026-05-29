import React, { useRef, useState } from 'react';
import { Button, Checkbox, ItemDataType, Search, Tag } from '@appkit4/react-components';
import classNames from 'classnames';
import Trigger from '@appkit4/react-components/trigger';
import { useClickOutside } from '@appkit4/react-components/utils';
import './datalist-header.scss';

const autoAdjustOverflowTopBottom = {
    shiftX: 64,
    adjustY: 1,
};
const targetOffset = [0, 0];
const builtingPlacements = {
    bottomRight: {
        points: ['tr', 'br'],
        overflow: autoAdjustOverflowTopBottom,
        offset: [0, 4],
        targetOffset,
    }
};

const getNextFocusableElement = (findElement: HTMLElement | Document, element: HTMLElement, isPre = false, getFirst = false) => {
    const controls = `button:not([tabindex = "-1"]):not([disabled]):not([style*="display:none"]):not([hidden]),
    [href][clientHeight][clientWidth]:not([tabindex = "-1"]):not([disabled]):not([style*="display:none"]):not([hidden]),
    input:not([tabindex = "-1"]):not([disabled]):not([style*="display:none"]):not([hidden]),
    select:not([tabindex = "-1"]):not([disabled]):not([style*="display:none"]):not([hidden]),
    textarea:not([tabindex = "-1"]):not([disabled]):not([style*="display:none"]):not([hidden]),
    [tabIndex]:not([tabIndex = "-1"]):not([disabled]):not([style*="display:none"]):not([hidden]),
    [contenteditable]:not([tabIndex = "-1"]):not([disabled]):not([style*="display:none"]):not([hidden])`;
    let focusable = findElement?.querySelectorAll(controls);//(containerRef.current! as HTMLElement)?
    const index = Array.prototype.indexOf.call(Array.from(focusable), element);
    return getFirst ? focusable[0] : Array.from(focusable)[isPre ? (index - 1) : (index + 1)];
}

export interface DataListHeaderProps {
    condition?: string;
    renderIcons?: React.ReactNode;
    renderTemplate?: React.ReactNode;
    selectedStr?: string;
    filterList?: ItemDataType[];
    handleSeachChange?: (value: string, event: React.SyntheticEvent) => void;
    className?: string;
    solid?: boolean;
    compactFilter?: boolean;
}
const DataListHeader = React.forwardRef<HTMLElement, DataListHeaderProps>((props: DataListHeaderProps, ref) => {
    const blurEvent = (event: React.SyntheticEvent) => (event.target as HTMLElement).classList.remove('keyboard-focus');
    const {
        renderIcons,
        selectedStr = '',
        handleSeachChange,
        filterList,
        renderTemplate = <Button kind='secondary' className="ap-pattern-btn-compact-view" onBlur={blurEvent}>
        <span className="Appkit4-icon icon-menu-outline"></span><span>Compact view</span>
        </Button>,
        className,
        compactFilter = false,
        solid = false
    } = props;

    const [tags, setTags] = React.useState<string[]>([]);
    const containerRef = useRef<any>(null);
    const btnRef = useRef<any>(null);
    const headerRef = useRef(null);
    const isEnterRef = useRef(false);
    const [hidValue, setHidValue] = useState(false);

    const onKeyDownClear = (event: React.KeyboardEvent<HTMLDivElement>) => {
        const { key } = event;
        if (key === 'Enter') {
            setTags([]);
        }
    }

    const onKeyDownHeader = (event: React.KeyboardEvent<HTMLDivElement>) => {
        const { key } = event;
        const element = document.activeElement as HTMLElement;
        if (key === 'Tab') {
            if ((element.classList.contains('ap-pattern-btn-filters')
                || element.classList.contains('ap-pattern-btn-compact-view')
                || element.classList.contains('ap-pattern-btn-clear-filters')
                || element.classList.contains('ap-pattern-btn-apply')) &&
                !element.classList.contains('keyboard-focus')) {
                element.classList.add('keyboard-focus');
            }
        } else if (key === 'Enter') {
            if (element.classList.contains('ap-pattern-btn-filters') && !element.classList.contains('keyboard-focus'))
                element.classList.add('keyboard-focus');
        }
    }

    const FilterTemplate = () => {
        const [showMenu, setShowMenu] = useState(false);
        const [selectedFilter, setSelectedFilter] = React.useState<Array<string>>([]);

        const handleSelection = (itemName: string, filterList: ItemDataType[] | undefined) => {
            let nextselectedFilter = Array.from(selectedFilter)
            if (nextselectedFilter.indexOf(itemName) === -1) {
                nextselectedFilter.push(itemName)
            } else {
                nextselectedFilter.splice(nextselectedFilter.indexOf(itemName), 1);
            }
            const selectedList = filterList?.filter(item => nextselectedFilter.includes(item.primary)).map(item => item.primary) as string[];
            setSelectedFilter(selectedList);
        }

        React.useEffect(() => {
            setSelectedFilter(tags);
        }, [showMenu])

        React.useEffect(() => {
            if (isEnterRef.current) {
                ((headerRef.current! as HTMLElement).querySelector('.ap-pattern-btn-filters') as HTMLElement)?.focus();
                isEnterRef.current = false;
            }
        }, [tags])

        React.useEffect(() => {
            if(navigator.platform.toUpperCase().indexOf('MAC') >= 0)
                setHidValue(true);
        }, [])

        useClickOutside({
            enabled: true,
            isOutside: event => !containerRef.current?.contains(event.target as HTMLElement) && !btnRef.current?.contains(event.target as HTMLElement),
            handle: () => setShowMenu(false)
        })

        const onKeyDown = (event: React.KeyboardEvent<HTMLDivElement>) => {
            if (!containerRef.current) return;
            const element = document.activeElement as HTMLElement;
            const nextEle = getNextFocusableElement(containerRef.current! as HTMLElement, element);
            const preEle = getNextFocusableElement(containerRef.current! as HTMLElement, element, true);
            const btnFilter = (headerRef.current! as HTMLElement).querySelector('.ap-pattern-btn-filters') as HTMLElement;
            const btnNext = getNextFocusableElement(headerRef.current! as HTMLElement, btnFilter!);
            const { key, shiftKey } = event;
            if (key === 'Tab') {
                if (!element.classList.contains('ap-pattern-btn-filters')) {
                    event.preventDefault();
                } else {
                    const first = (getNextFocusableElement(containerRef.current! as HTMLElement, element, false, true) as HTMLElement);
                    first.focus();
                    if (showMenu) event.preventDefault();
                    return;
                }
                if (shiftKey) {
                    if (preEle) {
                        (preEle as HTMLElement)?.focus();
                    } else {
                        if (btnFilter) {
                            btnFilter.focus();
                            setShowMenu(false);
                        }
                    }
                } else {
                    if (nextEle) {
                        (nextEle as HTMLElement)?.focus();
                    } else {
                        if (btnFilter) {
                            (btnNext as HTMLElement)?.focus();
                            setShowMenu(false);
                        }
                    }
                }
            } else if (key === 'Enter') {
                event.preventDefault();
                const list = (containerRef.current! as HTMLElement).querySelectorAll('.filter-list-row');
                const index = Array.from(list).indexOf(element);
                index > -1 && handleSelection(filterList?.[index]?.primary, filterList);
                if (!showMenu) setShowMenu(true);
                if (showMenu && element.className.indexOf('btn-apply') > -1) {
                    setShowMenu(false);
                    btnFilter.focus();
                    isEnterRef.current = true;
                }
            }
        }

        return (
            <div onKeyDown={onKeyDown}>
                <Trigger autoDestroy
                    builtinPlacements={builtingPlacements}
                    popupVisible={showMenu}
                    action="click" popupPlacement="bottomRight"
                    popup={<div className="ap-pattern-filter-popup" ref={containerRef}>
                        <div className={classNames('filter-content', className, { 'empty': !filterList || filterList.length === 0 })} aria-hidden={hidValue}>
                            {
                                filterList?.map((item, index: number) => <div tabIndex={0} className={classNames("filter-list-row")} key={index}>
                                    <Checkbox checked={selectedFilter.indexOf(item.primary) > -1} tabIndex={-1} onChange={() => { handleSelection(filterList?.[index].primary, filterList) }}>{item.primary}</Checkbox>
                                    <div className="secondary-text">{item.secondary}</div>
                                </div>)
                            }
                        </div>
                        <div className="filter-footer">
                            <Button kind='secondary' className="ap-pattern-btn-clear-filters" onClick={() => { setSelectedFilter([]) }}>Clear filters</Button>
                            <Button className="ap-pattern-btn-apply" onClick={() => {
                                setTags(selectedFilter);
                                setShowMenu(false);
                            }}>Apply</Button>
                        </div>
                    </div>}>
                    <Button aria-expanded={showMenu} ref={btnRef} kind='secondary' className={classNames('ap-pattern-btn-filters', { 'show-menu': showMenu, 'with-count': tags.length > 0, 'compact': compactFilter })} onClick={() => setShowMenu(val => !val)}><span className="Appkit4-icon icon-filter-outline"></span>
                        Filters{tags.length > 0 && <span className='filters-count'>({tags.length})</span>}<span className={classNames("Appkit4-icon icon-down-chevron-outline", { 'rotate': showMenu })} aria-hidden></span></Button>
                </Trigger>
            </div>
        );
    }

    return <div ref={headerRef} className={classNames('ap-pattern-data-list-header', className, {
        'solid': solid
    })} onKeyUp={onKeyDownHeader}>
        <div className="data-list-header-filter" >
            <Search searchType={"secondary"} onChange={handleSeachChange} />
            <div className="data-list-header-right-filter">
                <FilterTemplate></FilterTemplate>
                {renderTemplate}
            </div>
        </div>
        <div className="data-list-header-action-filter">
            <div className="data-list-header-selected">
                <div className="header-selected-text">{selectedStr}</div>
                <div className="icon-panel">
                    {renderIcons}
                </div>
            </div>
            <div className="data-list-header-right-option">
                <div className="tags-panel">
                    {tags.map((item: string, index: number) => <Tag key={index} onClose={() => setTags(tags.filter(itemInfo => itemInfo !== item))}>{item}</Tag>)}
                    <div className="tags-panel-border"></div>
                </div>
                <div tabIndex={0} className="header-right-option-clear" onKeyDown={onKeyDownClear} onClick={() => setTags([])}>Clear filters</div>
            </div>
        </div>
    </div>
});

export default DataListHeader

